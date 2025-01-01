using Chatty.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Chatty.Domain;

public static class DomainConfigurator
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddCoreDomain<IDomainMarker>();
        return services;
    }
}