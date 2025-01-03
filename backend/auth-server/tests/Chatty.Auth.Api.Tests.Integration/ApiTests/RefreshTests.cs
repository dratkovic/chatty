using System.Net;
using System.Text;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Login;
using Chatty.Authentication.Api.Features.User.Refresh;
using Chatty.Authentication.Api.Features.User.Register;
using Chatty.Core.Contracts.Routes;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class RefreshTests: IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public RefreshTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }
    
     [Fact]
    public async Task Refresh_ShouldReturnOkAndANewToken_WhenValidRefreshToken()
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
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Login, loginRequest);
        var authResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Act
        var refreshTokenRequest = new RefreshRequest(authResponse!.Token, authResponse!.RefreshToken);
        var refreshResponse = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Refresh, refreshTokenRequest);
        var refreshAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        refreshAuthResponse!.Token.Should().NotBeNullOrEmpty();
        refreshAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();
    } 
    
    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenInvalidRefreshToken()
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
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Login, loginRequest);
        var authResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Act
        var refreshTokenRequest = new RefreshRequest(authResponse!.Token, "invalid_refresh_token");
        var refreshResponse = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Refresh, refreshTokenRequest);
        
        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    } 
    
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
    
}