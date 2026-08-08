using System.Net;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Pablo.API.Features.Auth;
using Pablo.API.Infrastructure.Identity;

namespace Pablo.API.Tests.Features.Auth;

public class AuthControllerTests(PabloApiFactory factory) : IClassFixture<PabloApiFactory>
{
    private const string ValidPassword = "Password1!";

    public static TheoryData<string?, string?, string, string> InvalidLoginCases => new()
    {
        // Missing / empty
        { "test@example.com", "", "Password", InvalidLoginRequestException.Required("Password") },
        { "", ValidPassword, "Email", InvalidLoginRequestException.Required("Email") },
        { "test@example.com", null, "Password", InvalidLoginRequestException.Required("Password") },
        { null, ValidPassword, "Email", InvalidLoginRequestException.Required("Email") },

        // Whitespace
        { "   ", ValidPassword, "Email", InvalidLoginRequestException.Whitespace("Email") },
        { "test@example.com", "        ", "Password", InvalidLoginRequestException.Whitespace("Password") },

        // Invalid email format
        { "not-an-email", ValidPassword, "Email", InvalidLoginRequestException.InvalidEmail() },
        { "missing-domain@", ValidPassword, "Email", InvalidLoginRequestException.InvalidEmail() },
        { "@example.com", ValidPassword, "Email", InvalidLoginRequestException.InvalidEmail() },
    };

    [Fact]
    public async Task Post_login_returns_ok()
    {
        var client = factory.CreateClient();
        var email = $"ok-{Guid.NewGuid():N}@example.com";
        const string displayName = "John Doe";
        await SeedUserAsync(email, ValidPassword, displayName);

        var loginRequest = new LoginRequest(Email: email, Password: ValidPassword);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponseBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(loginResponseBody);
        Assert.Equal(email, loginResponseBody.Email);
        Assert.Equal(displayName, loginResponseBody.DisplayName);
    }

    [Fact]
    public async Task Post_login_returns_unauthorized_when_password_is_wrong()
    {
        var client = factory.CreateClient();
        var email = $"wrong-pw-{Guid.NewGuid():N}@example.com";
        await SeedUserAsync(email, ValidPassword, "John Doe");

        var loginRequest = new LoginRequest(Email: email, Password: "WrongPassword1!");
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var problem = await loginResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        var expected = new InvalidCredentialsException();

        Assert.Equal((HttpStatusCode)expected.StatusCode, loginResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(expected.Detail, problem.Detail);
        Assert.Equal(expected.Title, problem.Title);
        Assert.Equal(expected.StatusCode, problem.Status);
    }

    [Fact]
    public async Task Post_login_returns_unauthorized_when_email_is_unknown()
    {
        var client = factory.CreateClient();
        var loginRequest = new LoginRequest(
            Email: $"unknown-{Guid.NewGuid():N}@example.com",
            Password: ValidPassword);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var problem = await loginResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        var expected = new InvalidCredentialsException();

        Assert.Equal((HttpStatusCode)expected.StatusCode, loginResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(expected.Detail, problem.Detail);
        Assert.Equal(expected.Title, problem.Title);
        Assert.Equal(expected.StatusCode, problem.Status);
    }

    [Theory]
    [MemberData(nameof(InvalidLoginCases))]
    public async Task Post_login_returns_error_when_request_is_invalid(
        string? email,
        string? password,
        string expectedField,
        string expectedMessage)
    {
        var client = factory.CreateClient();
        var loginRequest = new LoginRequest(Email: email!, Password: password!);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var problem = await loginResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var expected = InvalidLoginRequestException.For(expectedField, expectedMessage);

        Assert.Equal((HttpStatusCode)expected.StatusCode, loginResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(expected.Detail, problem.Detail);
        Assert.Equal(expected.Title, problem.Title);
        Assert.Equal(expected.StatusCode, problem.Status);
        Assert.Equal(expected.Errors, problem.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("{")]
    [InlineData("{not json}")]
    [InlineData("null")]
    public async Task Post_login_returns_bad_request_when_body_is_malformed_or_empty(string body)
    {
        var client = factory.CreateClient();
        using var content = new StringContent(body, Encoding.UTF8, "application/json");

        var loginResponse = await client.PostAsync("/api/auth/login", content);

        Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
    }

    private async Task SeedUserAsync(string email, string password, string displayName)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthenticationUser>>();

        var result = await userManager.CreateAsync(
            new AuthenticationUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
            },
            password);

        Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}