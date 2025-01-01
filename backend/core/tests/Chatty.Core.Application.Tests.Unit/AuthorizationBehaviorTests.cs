using Chatty.Core.Application.Behaviors;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Models;
using ErrorOr;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Chatty.Core.Application.Tests.Unit;

public class AuthorizationBehaviorTests
{
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
    private readonly AuthorizationBehavior<TestRequest, ErrorOr<string>> _behavior;

    public AuthorizationBehaviorTests()
    {
        _authenticatedUserProvider = Substitute.For<IAuthenticatedUserProvider>();
        _behavior = new AuthorizationBehavior<TestRequest, ErrorOr<string>>(_authenticatedUserProvider);
    }

    [Fact]
    public async Task Handle_WithoutAuthorizationAttribute_ShouldCallNext()
    {
        // Arrange
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<string>>>();
        next().Returns("Success");

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("Success");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WithAuthorizationAttributeAndAuthenticatedUser_ShouldCallNext()
    {
        // Arrange
        var request = new AuthorizedTestRequest();
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<string>>>();
        next().Returns("Success");

        var authenticatedUser = new AuthenticationUser("1", "user@example.com", "John", "Doe", new List<string>());
        _authenticatedUserProvider.GetCurrentUser().Returns(authenticatedUser);

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("Success");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WithAuthorizationAttributeAndGuestUser_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var request = new AuthorizedTestRequest();
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<string>>>();

        _authenticatedUserProvider.GetCurrentUser().Returns(AuthenticationUser.GuestUser);

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unauthorized);
        result.FirstError.Description.Should().Be("User is forbidden from taking this action");
        await next.DidNotReceive().Invoke();
    }

    private class TestRequest : IRequest<ErrorOr<string>> { }

    [DAuthorize]
    private class AuthorizedTestRequest : TestRequest{ }
}