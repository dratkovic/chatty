using System.Net;
using System.Text;
using Bogus;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Register;
using Chatty.Core.Contracts.Routes;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class ConfirmEmailTests: IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public ConfirmEmailTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }
    
    [Fact]
    public async Task ConfirmEmail_ShouldReturnOk_WhenValidCode()
    {
        // Arrange
        var request = _faker.Generate();
        _factory.EmailSender.SendConfirmationLinkAsync(Arg.Any<AppUser>(),
                Arg.Any<string>())
            .Returns(Task.CompletedTask);
        var authResponse = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByEmailAsync(request.Email);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        
        // Act
        var response = await _factory.HttpClient.GetAsync($"{AuthenticationApiRoutes.Authentication.ConfirmEmail}?userId={user.Id}&code={code}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ConfirmEmail_ShouldReturnUnauthorized_WhenInvalidCode()
    {
        // Arrange
        var request = _faker.Generate();
        _factory.EmailSender.SendConfirmationLinkAsync(Arg.Any<AppUser>(),
                Arg.Any<string>())
            .Returns(Task.CompletedTask);
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByEmailAsync(request.Email);
        
        // Act
        var response = await _factory.HttpClient.GetAsync($"{AuthenticationApiRoutes.Authentication.ConfirmEmail}?userId={user.Id}&code=invalid");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
    
}