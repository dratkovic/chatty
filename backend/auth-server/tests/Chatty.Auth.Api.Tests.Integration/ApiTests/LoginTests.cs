using System.Net;
using System.Text;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Login;
using Chatty.Authentication.Api.Features.User.Register;
using Chatty.Core.Contracts.Routes;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class LoginTests: IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public LoginTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }
    

    [Fact]
    public async Task Login_ShouldReturnValidationError_WhenEmailNotConfirmed()
    {
        // Arrange
        var request = _faker.Generate();
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        
        var loginRequest = new LoginRequest(request.Email, request.Password);
        
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Login, loginRequest);
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be("General:Authorization");
        error.description.Should().Be("Account is not confirmed");
    }
    
    [Fact]
    public async Task Login_ShouldReturnValidationError_WhenInvalidLogin()
    {
        // Arrange
        var request = _faker.Generate();
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        
        var loginRequest = new LoginRequest(request.Email, "invalid");
        
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Login, loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Login_ShouldReturnAuthenticationResponse_WhenValidLogin()
    {
        // Arrange
        var request = _faker.Generate();
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByEmailAsync(request.Email);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        await _factory.HttpClient.GetAsync($"{AuthenticationApiRoutes.Authentication.ConfirmEmail}?userId={user.Id}&code={code}");
        
        var loginRequest = new LoginRequest(request.Email, request.Password);
        
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Login, loginRequest);
        var authResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        authResponse!.FirstName.Should().Be(request.FirstName);
        authResponse.LastName.Should().Be(request.LastName);
        authResponse.Email.Should().Be(request.Email.ToLower());
        authResponse.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }

    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
    
}