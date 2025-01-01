namespace Sensei.Core.Contracts.Subscriptions;

public record UpdateUserProfileRequest(
    string UserId,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    string Pseudonym,
    DateTime DateOfBirth,
    List<string> Instruments,
    List<string> Genres
) { }