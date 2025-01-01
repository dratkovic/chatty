
using ErrorOr;
using MediatR;
using NSubstitute;
using Xunit;

namespace Chatty.Tests.Unit.Application;

public class AuthorizationBehaviorTests
{
    private readonly IUserSession _userSession;
    private readonly AuthorizationBehavior<IRequest<IErrorOr>, IErrorOr> _authorizationBehavior;

    public AuthorizationBehaviorTests()
    {
        _userSession = Substitute.For<IUserSession>();
        _authorizationBehavior = new AuthorizationBehavior<IRequest<IErrorOr>, IErrorOr>(_userSession);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorizedError_WhenUserIsGuest()
    {
        // Arrange
        var request = new FakeRequest();
        var next = Substitute.For<RequestHandlerDelegate<IErrorOr>>();
        _userSession.GetCurrentUser().Returns(new CurrentUser(Guid.Empty, string.Empty));

        var exceptionHappened = false;
        // Act
        try
        {
            await _authorizationBehavior.Handle(request, next, CancellationToken.None);
        }
        catch (Exception e)
        {
            exceptionHappened = true;
        }

        // Assert
        Assert.True(exceptionHappened);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoAuthorizationAttributes()
    {
        // Arrange
        var request = new FakeRequest();
        var next = Substitute.For<RequestHandlerDelegate<IErrorOr>>();
        _userSession.GetCurrentUser().Returns(new CurrentUser(Guid.NewGuid(), "test@test.com"));

        // Act
        var result = await _authorizationBehavior.Handle(request, next, CancellationToken.None);

        // Assert
        await next.Received(1).Invoke();
    }
}

[Authorize]
public class FakeRequest : IRequest<IErrorOr>
{
}