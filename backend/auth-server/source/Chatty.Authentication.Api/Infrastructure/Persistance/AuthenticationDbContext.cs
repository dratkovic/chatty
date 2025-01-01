using Chatty.Authentication.Api.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Authentication.Api.Infrastructure;

public class AuthenticationDbContext : IdentityDbContext<AppUser>
{
    protected AuthenticationDbContext()
    {
    }

    public AuthenticationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("auth");

        builder.Entity<AppUser>()
            .Property(x => x.FirstName)
            .HasMaxLength(32);
        
        builder.Entity<AppUser>()
            .Property(x => x.LastName)
            .HasMaxLength(32);
        
        builder.Entity<AppUser>()
            .Property(x => x.RefreshToken)
            .HasMaxLength(1024);
        
        base.OnModelCreating(builder);  
    }
}