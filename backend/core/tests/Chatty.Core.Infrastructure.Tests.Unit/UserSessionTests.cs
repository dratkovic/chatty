using System.Security.Claims;
using Chatty.Core.Infrastructure.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace Sensei.Core.Infrastructure.Tests.Unit;

public class AuthenticatedUserProviderTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticatedUserProvider _authenticatedUserProvider;

    public AuthenticatedUserProviderTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _authenticatedUserProvider = new AuthenticatedUserProvider(_httpContextAccessor);
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnGuestUser_WhenHttpContextIsNull()
    {
        _httpContextAccessor.HttpContext.Returns((HttpContext)null!);

        var result = _authenticatedUserProvider.GetCurrentUser();

        result.IsGuest.Should().BeTrue();
        result.Id.Should().BeEmpty();
        result.Email.Should().Be("some@invalid.email");
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnGuestUser_WhenNoClaims()
    {
        var httpContext = new DefaultHttpContext();
        _httpContextAccessor.HttpContext.Returns(httpContext);

        var result = _authenticatedUserProvider.GetCurrentUser();

        result.IsGuest.Should().BeTrue();
        result.Id.Should().BeEmpty();
        result.Email.Should().Be("some@invalid.email");
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnAuthenticatedUser_WhenClaimsArePresent()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "john@example.com"),
            new Claim(ClaimTypes.Name, "John"),
            new Claim(ClaimTypes.Surname, "Doe"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        var result = _authenticatedUserProvider.GetCurrentUser();

        result.IsGuest.Should().BeFalse();
        result.Id.Should().Be("123");
        result.Email.Should().Be("john@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Roles.Should().BeEquivalentTo(new List<string> { "User", "Admin" });
    }
}