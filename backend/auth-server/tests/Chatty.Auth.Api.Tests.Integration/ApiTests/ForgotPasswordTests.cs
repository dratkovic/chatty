using System.Net;
using System.Text;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.ForgotPassword;
using Chatty.Authentication.Api.Features.User.Register;
using Chatty.Core.Contracts.Routes;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class ForgotPasswordTests: IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public ForgotPasswordTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }
    
    [Fact]
    public async Task ForgotPassword_ShouldReturnOk_WhenValidConfirmedEmail()
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
        _factory.EmailSender.SendPasswordResetCodeAsync(Arg.Any<AppUser>(),
                Arg.Any<string>())
            .Returns(Task.CompletedTask);
        
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.ForgotPassword, new ForgotPasswordRequest(request.Email));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        await _factory.EmailSender.Received(1).SendPasswordResetCodeAsync(
            Arg.Is<AppUser>(u => u.Email == request.Email),
            Arg.Any<string>());
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
    
}