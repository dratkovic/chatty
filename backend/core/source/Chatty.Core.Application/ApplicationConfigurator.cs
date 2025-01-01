using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sensei.Core.Application.Behaviors;
using Sensei.Core.Application.Common.Interfaces;
using Sensei.Core.Application.Mapping;

namespace Sensei.Core.Application;

public static class ApplicationConfigurator
{
    public static IServiceCollection AddCoreApplication<T>(
        this IServiceCollection services)
    {
        var assembly = typeof(T).Assembly;
        
        services.AddValidatorsFromAssemblyContaining<T>();
        
        return services
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            })
            .AddAutoMapperProfile(assembly);
    }

    private static IServiceCollection AddAutoMapperProfile(
        this IServiceCollection services, Assembly assembly)
        => services
            .AddAutoMapper(
                (_, config) => config
                    .AddProfile(new MappingProfile(assembly)),
                Array.Empty<Assembly>());
    
  
}