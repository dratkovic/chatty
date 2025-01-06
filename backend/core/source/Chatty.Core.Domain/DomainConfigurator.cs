using System.Reflection;
using Chatty.Core.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Chatty.Core.Domain;

public static class DomainConfigurator
{
    public static IServiceCollection AddCoreDomain<T>(
        this IServiceCollection services)
        => services
            .AddInitialData(typeof(T).Assembly);
    
    private static IServiceCollection AddInitialData(
        this IServiceCollection services,
        Assembly assembly)
        => services
            .Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(IInitialData<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
}