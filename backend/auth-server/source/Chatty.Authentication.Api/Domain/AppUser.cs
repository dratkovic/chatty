using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Domain;

public class AppUser : IdentityUser
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}