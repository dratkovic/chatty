using Chatty.Core.Infrastructure.Authentication.PasswordHasher;
using FluentAssertions;
using Xunit;

namespace Sensei.Core.Infrastructure.Tests.Unit;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Theory]
    [InlineData("weak")]
    [InlineData("123456")]
    [InlineData("password")]
    public void HashPassword_ShouldReturnError_WhenPasswordTooWeak(string weakPassword)
    {
        var result = _passwordHasher.HashPassword(weakPassword);

        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Be("Password too weak");
    }

    [Theory]
    [InlineData("StrongP@ssw0rd")]
    [InlineData("C0mpl3x!P@ssw0rd")]
    [InlineData("S3cur3&P@ssw0rd123")]
    public void HashPassword_ShouldReturnHashedPassword_WhenPasswordIsStrong(string strongPassword)
    {
        var result = _passwordHasher.HashPassword(strongPassword);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().StartWith("$2a$");
    }

    [Fact]
    public void IsCorrectPassword_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        var password = "StrongP@ssw0rd";
        var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        var result = _passwordHasher.IsCorrectPassword(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsCorrectPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
    {
        var password = "StrongP@ssw0rd";
        var hash = BCrypt.Net.BCrypt.EnhancedHashPassword("DifferentP@ssw0rd");

        var result = _passwordHasher.IsCorrectPassword(password, hash);

        result.Should().BeFalse();
    }
}