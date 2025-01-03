using System.Net;
using Bogus;
using Chatty.Authentication.Api.Features.User.Register;
using FluentAssertions;
using NSubstitute;
using Chatty.Authentication.Api.Domain;
using Chatty.Core.Contracts.Routes;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class ResendConfirmationEmailTests : IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public ResendConfirmationEmailTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }

    [Fact]
    public async Task ResendConfirmationEmail_ShouldReturnOkAnSendAnEmail_WhenValidEmail()
    {
        // Arrange
        var request = _faker.Generate();
        _factory.EmailSender.SendConfirmationLinkAsync(Arg.Any<AppUser>(),
                Arg.Any<string>())
            .Returns(Task.CompletedTask);
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);

        // Act
        var response =
            await _factory.HttpClient.GetAsync($"{AuthenticationApiRoutes.Authentication.ResendConfirmEmail}?email={request.Email}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _factory.EmailSender.Received(2).SendConfirmationLinkAsync(
            Arg.Is<AppUser>(u => u.Email == request.Email),
            Arg.Any<string>());
    }

    [Fact]
    public async Task ResendConfirmationEmail_ShouldReturnOkAndNotSendEmail_WhenInvalidEmail()
    {
        // Arrange
        var request = _faker.Generate();
        _factory.EmailSender.SendConfirmationLinkAsync(Arg.Any<AppUser>(),
                Arg.Any<string>())
            .Returns(Task.CompletedTask);
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);

        // Act
        var response =
            await _factory.HttpClient.GetAsync(
                $"{AuthenticationApiRoutes.Authentication.ResendConfirmEmail}?email=invalid@invalid.com");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await _factory.EmailSender.Received(1).SendConfirmationLinkAsync(
            Arg.Is<AppUser>(u => u.Email == request.Email),
            Arg.Any<string>());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
}