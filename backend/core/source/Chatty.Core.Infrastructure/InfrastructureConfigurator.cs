using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sensei.Core.Application.Common.Interfaces;
using Sensei.Core.Application.Common.Persistance;
using Sensei.Core.Domain;
using Sensei.Core.Infrastructure.Authentication.PasswordHasher;
using Sensei.Core.Infrastructure.Authentication.TokenGenerator;
using Sensei.Core.Infrastructure.Common;
using Sensei.Core.Infrastructure.Persistance;

namespace Sensei.Core.Infrastructure;

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