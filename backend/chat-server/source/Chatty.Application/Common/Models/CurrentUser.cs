namespace Chatty.Application.Common.Models;

public record CurrentUser(
    Guid Id,
    string Email
    )
{
    public bool IsGuest => Id == Guid.Empty;

    public static CurrentUser GuestUser => new(Id: Guid.Empty, "some@invalid.email");
};