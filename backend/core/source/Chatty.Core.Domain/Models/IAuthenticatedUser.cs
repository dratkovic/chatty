namespace Chatty.Core.Domain.Models;

public interface IAuthenticatedUser
{
    string Id { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    List<string> Roles { get; }
    
    bool IsGuest => Id == String.Empty;
}