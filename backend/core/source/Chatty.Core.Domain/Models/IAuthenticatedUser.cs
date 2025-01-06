namespace Chatty.Core.Domain.Models;

public interface IAuthenticatedUser
{
    Guid Id { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    List<string> Roles { get; }
    
    bool IsGuest => Id == Guid.Empty;
}