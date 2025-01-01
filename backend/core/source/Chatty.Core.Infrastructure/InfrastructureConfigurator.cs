using System.Text;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Domain;
using Chatty.Core.Infrastructure.Authentication.PasswordHasher;
using Chatty.Core.Infrastructure.Authentication.TokenGenerator;
using Chatty.Core.Infrastructure.Common;
using Chatty.Core.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chatty.Core.Infrastructure;

public static class InfrastructureConfigurator
{
    public static IServiceCollection AddCoreInfrastructure<T>(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPaginatedRepository<>), typeof(PaginatedRepository<>));
        services.AddSingleton<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
        
        services.AddRepositories<T>();
        services.AddCommonImplementations<T>();

        return services;
    }

    public static IServiceCollection AddCoreInfrastructureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Section, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenManipulator, JwtTokenManipulator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            });

        services.AddAuthorization();
        return services;
    }
    
    internal static IServiceCollection AddCommonImplementations<T>(
        this IServiceCollection services)
        => services
            .Scan(scan => scan
                .FromAssemblies(typeof(T).Assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(IAuthenticatedUserProvider)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
    
    internal static IServiceCollection AddRepositories<T>(
        this IServiceCollection services)
        => services
            .Scan(scan => scan
                .FromAssemblies(typeof(T).Assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(IReadOnlyRepository<>))
                    .AssignableTo(typeof(IRepository<>))
                    .AssignableTo(typeof(IAppDbContext)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
}