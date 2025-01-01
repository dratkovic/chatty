namespace Sensei.Core.Application.Common.Models;

public record UserSession(
    UserProfile UserProfile,
    Subscription Subscription)
{
}

public record UserProfile(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsRegistrationComplete = false)
{
}

public record Subscription(string Name, DateTime ValidUntilUtc){}