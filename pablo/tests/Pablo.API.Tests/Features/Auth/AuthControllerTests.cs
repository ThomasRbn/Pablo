using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Pablo.API.Features.Auth;
using Pablo.API.Features.Auth.Exceptions;
using Pablo.API.Infrastructure.Identity;
using Pablo.API.Infrastructure.Persistence;

namespace Pablo.API.Tests.Features.Auth;

public class AuthControllerTests(PabloApiFactory factory) : IClassFixture<PabloApiFactory>
{
    private const string ValidPassword = "Password1!";

    public static TheoryData<string?, string?, string, string> InvalidLoginCases => new()
    {
        // Missing / empty
        { "testuser", "", "Password", InvalidLoginRequestException.Required("Password") },
        { "", ValidPassword, "Username", InvalidLoginRequestException.Required("Username") },
        { "testuser", null, "Password", InvalidLoginRequestException.Required("Password") },
        { null, ValidPassword, "Username", InvalidLoginRequestException.Required("Username") },

        // Whitespace
        { "   ", ValidPassword, "Username", InvalidLoginRequestException.Whitespace("Username") },
        { "testuser", "        ", "Password", InvalidLoginRequestException.Whitespace("Password") },
    };

    [Fact]
    public async Task Post_login_returns_ok_with_email()
    {
        var client = factory.CreateClient();
        var email = $"ok-{Guid.NewGuid():N}@example.com";
        var username = $"user-{Guid.NewGuid():N}";
        const string displayName = "John Doe";
        var userId = await SeedUserAsync(username, email, ValidPassword, displayName);

        var loginRequest = new LoginRequest(Username: email, Password: ValidPassword);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponseBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(loginResponseBody);
        Assert.False(string.IsNullOrWhiteSpace(loginResponseBody.AccessToken));
        Assert.Equal("Bearer", loginResponseBody.TokenType);
        Assert.True(loginResponseBody.ExpiresIn > 0);
        Assert.Equal(username, loginResponseBody.Username);
        Assert.Equal(email, loginResponseBody.Email);
        Assert.Equal(displayName, loginResponseBody.DisplayName);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(loginResponseBody.AccessToken);
        Assert.Equal(userId, jwt.Subject);
        Assert.Equal(email, jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(displayName, jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task Post_login_returns_ok_with_username()
    {
        var client = factory.CreateClient();
        var email = $"ok-name-{Guid.NewGuid():N}@example.com";
        var username = $"user-{Guid.NewGuid():N}";
        const string displayName = "Jane Doe";
        await SeedUserAsync(username, email, ValidPassword, displayName);

        var loginRequest = new LoginRequest(Username: username, Password: ValidPassword);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponseBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(loginResponseBody);
        Assert.Equal(username, loginResponseBody.Username);
        Assert.Equal(email, loginResponseBody.Email);
        Assert.Equal(displayName, loginResponseBody.DisplayName);
    }

    [Fact]
    public async Task Post_login_returns_ok_for_seeded_root_user()
    {
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest(
            Username: DatabaseSeeder.RootUsername,
            Password: DatabaseSeeder.RootPassword);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponseBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(loginResponseBody);
        Assert.Equal(DatabaseSeeder.RootUsername, loginResponseBody.Username);
        Assert.Equal(DatabaseSeeder.RootEmail, loginResponseBody.Email);
        Assert.Equal(DatabaseSeeder.RootDisplayName, loginResponseBody.DisplayName);
    }

    [Fact]
    public async Task Post_login_returns_unauthorized_after_lockout_threshold()
    {
        var client = factory.CreateClient();
        var email = $"lockout-{Guid.NewGuid():N}@example.com";
        var username = $"lockout-{Guid.NewGuid():N}";
        await SeedUserAsync(username, email, ValidPassword, "John Doe");

        var maxFailedAttempts = await GetMaxFailedAccessAttemptsAsync();
        var wrongPasswordRequest = new LoginRequest(Username: email, Password: "WrongPassword1!");
        for (var attempt = 0; attempt < maxFailedAttempts; attempt++)
        {
            var failedResponse = await client.PostAsJsonAsync("/api/auth/login", wrongPasswordRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, failedResponse.StatusCode);
        }

        var lockedResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(Username: email, Password: ValidPassword));
        var problem = await lockedResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        var expected = new InvalidCredentialsException();

        Assert.Equal((HttpStatusCode)expected.StatusCode, lockedResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(expected.Detail, problem.Detail);
        Assert.Equal(expected.Title, problem.Title);
        Assert.Equal(expected.StatusCode, problem.Status);
    }

    [Fact]
    public async Task Post_login_returns_unauthorized_when_password_is_wrong()
    {
        var client = factory.CreateClient();
        var email = $"wrong-pw-{Guid.NewGuid():N}@example.com";
        var username = $"wrong-pw-{Guid.NewGuid():N}";
        await SeedUserAsync(username, email, ValidPassword, "John Doe");

        var loginRequest = new LoginRequest(Username: email, Password: "WrongPassword1!");
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
    public async Task Post_login_returns_unauthorized_when_username_is_unknown()
    {
        var client = factory.CreateClient();
        var loginRequest = new LoginRequest(
            Username: $"unknown-{Guid.NewGuid():N}",
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
        string? username,
        string? password,
        string expectedField,
        string expectedMessage)
    {
        var client = factory.CreateClient();
        var loginRequest = new LoginRequest(Username: username!, Password: password!);

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

    private async Task<int> GetMaxFailedAccessAttemptsAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthenticationUser>>();
        return userManager.Options.Lockout.MaxFailedAccessAttempts;
    }

    private async Task<string> SeedUserAsync(
        string username,
        string email,
        string password,
        string displayName)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthenticationUser>>();

        var user = new AuthenticationUser
        {
            UserName = username,
            Email = email,
            DisplayName = displayName,
        };
        var result = await userManager.CreateAsync(user, password);

        Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(e => e.Description)));
        return user.Id;
    }
}