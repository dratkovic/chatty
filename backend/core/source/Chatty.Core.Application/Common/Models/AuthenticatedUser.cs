using Chatty.Core.Domain.Models;

namespace Chatty.Core.Application.Common.Models;

public class AuthenticatedUser : IAuthenticatedUser
{
    public Guid Id { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public List<string> Roles { get; }

    public AuthenticatedUser(Guid id, string email, string firstName, string lastName, List<string> roles)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
    }

    public bool IsGuest => Id == Guid.Empty;

    public static AuthenticatedUser GuestUser => new(Guid.Empty, "some@invalid.email", "guest", "guest", []);

};