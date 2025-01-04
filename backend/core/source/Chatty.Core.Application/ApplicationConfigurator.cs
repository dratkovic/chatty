using System.Reflection;
using Chatty.Core.Application.Behaviors;
using Chatty.Core.Application.Mapping;
using FluentValidation;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;

namespace Chatty.Core.Application;

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

                cfg.NotificationPublisher = new TaskWhenAllPublisher();
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