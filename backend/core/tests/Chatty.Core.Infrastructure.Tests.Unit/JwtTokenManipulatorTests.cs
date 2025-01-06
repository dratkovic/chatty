using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using NSubstitute;
using Xunit;
using System.Security.Claims;
using Chatty.Core.Application.Common.Models;
using Chatty.Core.Infrastructure.Authentication.TokenGenerator;

namespace Chatty.Core.Infrastructure.Tests.Unit;

public class JwtTokenManipulatorTests
{
    private readonly JwtTokenManipulator _jwtTokenManipulator;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenManipulatorTests()
    {
        _jwtSettings = new JwtSettings
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            Secret = "VeryLongSecretKeyForTestingPurposesOnly",
            TokenExpirationInMinutes = 60,
            RefreshTokenExpirationInHours = 24
        };
        var options = Substitute.For<IOptions<JwtSettings>>();
        options.Value.Returns(_jwtSettings);
        _jwtTokenManipulator = new JwtTokenManipulator(options);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "test@example.com", "John", "Doe", new List<string> { "User" });

        var token = _jwtTokenManipulator.GenerateToken(user);

        token.Should().NotBeNullOrEmpty();
        var jwtToken = new JsonWebTokenHandler().ReadToken(token) as JsonWebToken;
        jwtToken.Should().NotBeNull();
        jwtToken!.Issuer.Should().Be(_jwtSettings.Issuer);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.FirstName);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Surname && c.Value == user.LastName);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnNonEmptyString()
    {
        var refreshToken = _jwtTokenManipulator.GenerateRefreshToken();

        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(20);
    }

    [Fact]
    public async Task GetPrincipalFromExpiredToken_ShouldReturnValidClaimsIdentity()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "test@example.com", "John", "Doe", new List<string> { "User" });
        var token = _jwtTokenManipulator.GenerateToken(user);

        var result = await _jwtTokenManipulator.GetPrincipalFromExpiredToken(token);

        result.IsError.Should().BeFalse();
        var claimsIdentity = result.Value;
        claimsIdentity.Should().NotBeNull();
        claimsIdentity.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        claimsIdentity.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        claimsIdentity.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.FirstName);
        claimsIdentity.Claims.Should().Contain(c => c.Type == ClaimTypes.Surname && c.Value == user.LastName);
        claimsIdentity.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public async Task GetPrincipalFromExpiredToken_ShouldReturnError_WhenTokenIsInvalid()
    {
        var invalidToken = "invalid.token.string";

        var result = await _jwtTokenManipulator.GetPrincipalFromExpiredToken(invalidToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Be("Invalid token");
    }
}