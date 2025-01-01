using Chatty.Application.Common.Interfaces;
using Chatty.Application.Common.Repositories;
using Chatty.Infrastructure.Common;
using Chatty.Infrastructure.Common.Persistence;
using Chatty.Infrastructure.Persistance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chatty.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddPersistence(configuration);
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IPaginatedRepository<>), typeof(PaginatedRepository<>));
        services.AddScoped<IChattyDbContext, ChattyDbContext>();
        // services.AddScoped<IPasswordHasher, PasswordHasher>();
        // services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IUserSession, UserSession>();

        return services;
    }

    // public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    // {
    //     var jwtSettings = new JwtSettings();
    //     configuration.Bind(JwtSettings.Section, jwtSettings);
    //
    //     services.AddSingleton(Options.Create(jwtSettings));
    //     services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    //     services.AddSingleton<IPasswordHasher, PasswordHasher>();
    //
    //     services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
    //         .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    //         {
    //             ValidateIssuer = true,
    //             ValidateAudience = true,
    //             ValidateLifetime = true,
    //             ValidateIssuerSigningKey = true,
    //             ValidIssuer = jwtSettings.Issuer,
    //             ValidAudience = jwtSettings.Audience,
    //             IssuerSigningKey = new SymmetricSecurityKey(
    //                 Encoding.UTF8.GetBytes(jwtSettings.Secret)),
    //         });
    //
    //     services.AddAuthorization();
    //     return services;
    // }

    public static IHostApplicationBuilder AddDb(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<ChattyDbContext>("chatty-db");

        return builder;
    }
}