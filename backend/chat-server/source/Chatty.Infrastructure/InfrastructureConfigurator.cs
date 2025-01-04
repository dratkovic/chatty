using Chatty.Application;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Infrastructure;
using Chatty.Domain;
using Chatty.Infrastructure.Persistance;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chatty.Infrastructure;

public static class InfrastructureConfigurator
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder app)
    {
        app.Services
            .AddDomain()
            .AddApplication()
            .AddCoreInfrastructure<IInfrastructureMarker>()
            .AddCoreInfrastructureJwtAuthentication(app.Configuration);

        app.AdPostgreSql();
        app.AddRedis();
        app.AddRabbitMq();

        app.AddHybridCache();

        return app;
    }

    public static IHostApplicationBuilder AddHybridCache(this IHostApplicationBuilder app)
    {
#pragma warning disable EXTEXP0018
        app.Services.AddHybridCache(o =>
        {
            o.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(10),
                Expiration = TimeSpan.FromMinutes(10),
            };
        });
#pragma warning restore EXTEXP0018
        
        return app;
    }
    
    public static IHostApplicationBuilder AddRabbitMq(this IHostApplicationBuilder app)
    {
        app.AddRabbitMQClient(connectionName: "chatty-rabbitmq");
        return app;
    }

    public static IHostApplicationBuilder AdPostgreSql(this IHostApplicationBuilder app)
    {
        app.AddNpgsqlDbContext<ChattyDbContext>("chatty-app-db");

        app.Services.AddScoped<IAppDbContext, ChattyDbContext>();

        return app;
    }

    public static IHostApplicationBuilder AddRedis(this IHostApplicationBuilder app)
    {
        app.AddRedisDistributedCache("chatty-redis");

        return app;
    }
}