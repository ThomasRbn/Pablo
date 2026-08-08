using System.Net;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using Pablo.API.Features.Auth;
using Pablo.API.Features.Auth.Types;

namespace Pablo.API.Tests.Features.Auth;

public class AuthControllerTests(PabloApiFactory factory) : IClassFixture<PabloApiFactory>
{
    private const string ValidPassword = "Password1!";

    public static TheoryData<string?, string?, string?, string, string> InvalidSignupCases => new()
    {
        // Missing / empty
        { "test@example.com", ValidPassword, "", "DisplayName", InvalidSignupRequestException.Required("DisplayName") },
        { "test@example.com", "", "John Doe", "Password", InvalidSignupRequestException.Required("Password") },
        { "", ValidPassword, "John Doe", "Email", InvalidSignupRequestException.Required("Email") },
        { "test@example.com", ValidPassword, null, "DisplayName", InvalidSignupRequestException.Required("DisplayName") },
        { "test@example.com", null, "John Doe", "Password", InvalidSignupRequestException.Required("Password") },
        { null, ValidPassword, "John Doe", "Email", InvalidSignupRequestException.Required("Email") },

        // Whitespace
        { "   ", ValidPassword, "John Doe", "Email", InvalidSignupRequestException.Whitespace("Email") },
        { "test@example.com", "        ", "John Doe", "Password", InvalidSignupRequestException.Whitespace("Password") },
        { "test@example.com", ValidPassword, "   ", "DisplayName", InvalidSignupRequestException.Whitespace("DisplayName") },

        // Invalid email format
        { "not-an-email", ValidPassword, "John Doe", "Email", InvalidSignupRequestException.InvalidEmail() },
        { "missing-domain@", ValidPassword, "John Doe", "Email", InvalidSignupRequestException.InvalidEmail() },
        { "@example.com", ValidPassword, "John Doe", "Email", InvalidSignupRequestException.InvalidEmail() },

        // Weak password (8+ chars, upper, lower, digit, symbol)
        { "test@example.com", "Pass1!", "John Doe", "Password", InvalidSignupRequestException.PasswordTooShort() },
        { "test@example.com", "password1!", "John Doe", "Password", InvalidSignupRequestException.PasswordMissingUppercase() },
        { "test@example.com", "PASSWORD1!", "John Doe", "Password", InvalidSignupRequestException.PasswordMissingLowercase() },
        { "test@example.com", "Password1", "John Doe", "Password", InvalidSignupRequestException.PasswordMissingSymbol() },
        { "test@example.com", "Password!", "John Doe", "Password", InvalidSignupRequestException.PasswordMissingDigit() },
    };

    [Fact]
    public async Task Post_signup_returns_ok()
    {
        var client = factory.CreateClient();
        var signupRequest = new SignupRequest(Email: "test@example.com", Password: ValidPassword, DisplayName: "John Doe");

        var signupResponse = await client.PostAsJsonAsync("/api/auth/signup", signupRequest);
        var signupResponseBody = await signupResponse.Content.ReadFromJsonAsync<SignupResponse>();

        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);
        Assert.NotNull(signupResponseBody);
        Assert.Equal(signupRequest.Email, signupResponseBody.Email);
        Assert.Equal(signupRequest.DisplayName, signupResponseBody.DisplayName);
    }

    [Fact]
    public async Task Post_signup_returns_error_when_email_duplicate()
    {
        var client = factory.CreateClient();

        var signupRequest = new SignupRequest(Email: "test@example.com", Password: ValidPassword, DisplayName: "John Doe");
        var signupRequestDuplicated = new SignupRequest(Email: "test@example.com", Password: "MyPassword1!", DisplayName: "Jane Doe");

        var signupResponse = await client.PostAsJsonAsync("/api/auth/signup", signupRequest);
        var signupResponseBody = await signupResponse.Content.ReadFromJsonAsync<SignupResponse>();

        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);
        Assert.NotNull(signupResponseBody);
        Assert.Equal(signupRequest.Email, signupResponseBody.Email);
        Assert.Equal(signupRequest.DisplayName, signupResponseBody.DisplayName);

        var signupResponseDuplicated = await client.PostAsJsonAsync("/api/auth/signup", signupRequestDuplicated);
        var problem = await signupResponseDuplicated.Content.ReadFromJsonAsync<ProblemDetails>();
        var expected = new DuplicateEmailException();

        Assert.Equal((HttpStatusCode)expected.StatusCode, signupResponseDuplicated.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(expected.Detail, problem.Detail);
        Assert.Equal(expected.Title, problem.Title);
        Assert.Equal(expected.StatusCode, problem.Status);
    }

    [Theory]
    [MemberData(nameof(InvalidSignupCases))]
    public async Task Post_signup_returns_error_when_request_is_invalid(
        string? email,
        string? password,
        string? displayName,
        string expectedField,
        string expectedMessage)
    {
        var client = factory.CreateClient();
        var signupRequest = new SignupRequest(Email: email!, Password: password!, DisplayName: displayName!);

        var signupResponse = await client.PostAsJsonAsync("/api/auth/signup", signupRequest);
        var problem = await signupResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var expected = InvalidSignupRequestException.For(expectedField, expectedMessage);

        Assert.Equal((HttpStatusCode)expected.StatusCode, signupResponse.StatusCode);
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
    public async Task Post_signup_returns_bad_request_when_body_is_malformed_or_empty(string body)
    {
        var client = factory.CreateClient();
        using var content = new StringContent(body, Encoding.UTF8, "application/json");

        var signupResponse = await client.PostAsync("/api/auth/signup", content);

        Assert.Equal(HttpStatusCode.BadRequest, signupResponse.StatusCode);
    }
}