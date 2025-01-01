using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sensei.Core.Domain.Models;

namespace Sensei.Core.Domain;

public static class DomainConfigurator
{
    public static IServiceCollection AddCoreDomain<T>(
        this IServiceCollection services)
        => services
            .AddFactories(typeof(T).Assembly)
            .AddInitialData(typeof(T).Assembly);

    private static IServiceCollection AddFactories(
        this IServiceCollection services,
        Assembly assembly)
        => services
            .Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(IFactory<>)))
                .AsMatchingInterface()
                .WithTransientLifetime());

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