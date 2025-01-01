using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Models;

public class AuthenticationUser : IAuthenticationUser
{
    public string Id { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public List<string> Roles { get; }

    public AuthenticationUser(string id, string email, string firstName, string lastName, List<string> roles)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
    }

    public bool IsGuest => Id == String.Empty;

    public static AuthenticationUser GuestUser => new(String.Empty, "some@invalid.email", "guest", "guest", []);

};