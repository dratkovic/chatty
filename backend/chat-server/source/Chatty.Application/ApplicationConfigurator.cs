using Chatty.Application.Common.Helpers;
using Chatty.Core.Application;
using Microsoft.Extensions.DependencyInjection;
namespace Chatty.Application;

public static class ApplicationConfigurator
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCoreApplication<IApplicationMarker>();
        
        services.AddScoped<IUserRetriever, UserRetriever>();
        
        return services;
    }
}