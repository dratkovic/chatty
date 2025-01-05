using Chatty.Application.Common.Helpers;
using Chatty.Application.Services;
using Chatty.Core.Application;
using Microsoft.Extensions.DependencyInjection;
namespace Chatty.Application;

public static class ApplicationConfigurator
{
    public static IServiceCollection AddApplication<T>(this IServiceCollection services)
    {
        services.AddCoreApplication<IApplicationMarker>();
        
        services.AddScoped<IUserRetriever, UserRetriever>();
        services.AddScoped<IMessageBroadcaster, MessageBroadcaster>();

        services.AddCommonImplementations<T>();
        
        return services;
    }
    
    internal static IServiceCollection AddCommonImplementations<T>(
        this IServiceCollection services)
        => services
            .Scan(scan => scan
                .FromAssemblies(typeof(T).Assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(ILiveChatService)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
    
}