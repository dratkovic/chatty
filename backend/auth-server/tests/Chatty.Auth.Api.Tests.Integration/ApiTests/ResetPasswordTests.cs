using System.Net;
using System.Text;
using Bogus;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Register;
using Chatty.Authentication.Api.Features.User.ResetPassword;
using Chatty.Core.Contracts.Routes;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class ResetPasswordTests: IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public ResetPasswordTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }
    
    

    [Fact]
    public async Task ResetPassword_ShouldReturnOk_WhenValidResetCode()
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
        var resetCode = await userManager.GeneratePasswordResetTokenAsync(user);
        resetCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetCode));
        
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.ResetPassword, new ResetPasswordRequest(user.Email!,"NewP@ssword12!", resetCode));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenInvalidResetCode()
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
        
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.ResetPassword, new ResetPasswordRequest(user.Email!,"NewP@ssword12!", "invalid_reset_code"));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
    
}