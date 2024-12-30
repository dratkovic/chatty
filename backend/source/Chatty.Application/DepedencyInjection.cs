using Chatty.Application.Behaviors;
using Chatty.Application.Common.Helpers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Chatty.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserRetriever, UserRetriever>();
        
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(IChattyApplicationMarker));

            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining(typeof(IChattyApplicationMarker));

        return services;
    }
}