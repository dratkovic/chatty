using System.Collections;
using System.Net;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Register;
using Chatty.Core.Contracts.Routes;

namespace Chatty.Auth.Api.Tests.Integration.ApiTests;

[Collection("Authentication tests")]
public class RegisterTests: IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly Faker<RegisterRequest> _faker;

    public RegisterTests(AuthApiFactory factory)
    {
        _factory = factory;
        _faker = UserFaker.GetRegisterFaker();
    }
    
    [Fact]
    public async Task Register_ShouldReturnCreatedWhenValidRequest()
    {
        // Arrange
        var request = _faker.Generate();

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Theory]
    [ClassData(typeof(RegisterUnvalidFaker))]
    public async Task Register_ShouldReturnBadRequestWhenInvalidRequest(RegisterRequest request, string code)
    {
        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be(code);
    }
    
    [Fact]
    public async Task Register_ShouldReturnBadRequestWhenEmailExists()
    {
        // Arrange
        var request = _faker.Generate();
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be("DuplicateUserName");
    }
    
    [Fact]
    public async Task Register_ShouldReturnBadRequestWhenEmailExistsCaseInsensitive()
    {
        // Arrange
        var request = _faker.Generate();
        await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, new RegisterRequest(request.Email.ToUpper(), request.Password, request.FirstName, request.LastName));
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be("DuplicateUserName");
    }
    
    [Fact]
    public async Task Register_ShouldSendConfirmationEmail()
    {
        // Arrange
        var request = _faker.Generate();
        _factory.EmailSender.SendConfirmationLinkAsync(Arg.Any<AppUser>(),
                Arg.Any<string>())
            .Returns(Task.CompletedTask);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(AuthenticationApiRoutes.Authentication.Register, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
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

public class RegisterUnvalidFaker : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var fake = UserFaker.GetRegisterFaker().Generate();
        
        yield return new object[] {new RegisterRequest("", fake.Password, fake.FirstName, fake.LastName), "Email"};
        yield return new object[] {new RegisterRequest(fake.Email, "", fake.FirstName, fake.LastName), "Password"};
        yield return new object[] {new RegisterRequest(fake.Email, fake.Password, "", fake.LastName), "FirstName"};
        yield return new object[] {new RegisterRequest(fake.Email, fake.Password, fake.FirstName, ""), "LastName"};
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}