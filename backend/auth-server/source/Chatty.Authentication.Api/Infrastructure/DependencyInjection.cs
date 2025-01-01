using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Infrastructure.Email;
using Chatty.Core.Api;
using Chatty.Core.Application;
using Chatty.Core.Domain;
using Chatty.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreDomain<IAuthenticationApiMarker>()
            .AddCoreApplication<IAuthenticationApiMarker>()
            .AddCoreInfrastructure<IAuthenticationApiMarker>()
            .AddCoreInfrastructureJwtAuthentication(configuration)
            .AddCoreWebApi();

        services.AddSingleton<IEmailSender, FakeEmailSender>();
            
        services.AddHttpContextAccessor();
        
        services.AddSwaggerGen();
        
        services.AddIdentity();

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        });

        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);
        
        return services;
    }
}