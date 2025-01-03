using Chatty.Application;
using Chatty.Core.Infrastructure;
using Chatty.Domain;
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
        
        //app.AddMongoDb();
        app.AddRedis();
            
        return app;
    }
    
    // public static IHostApplicationBuilder AddMongoDb(this IHostApplicationBuilder app)
    // {
    //     app.AddMongoDBClient("sensei-db");
    //     app.Services.AddScoped<IAppDbContext, SenseiDbContext>();
    //
    //     var connectionString = app.Configuration.GetConnectionString("sensei-db");
    //     app.Services.AddDbContext<SenseiDbContext>(options =>
    //         options.UseMongoDB(connectionString ?? "", "sensei"));
    //
    //     app.Services.AddScoped<IAppDbContext, SenseiDbContext>();
    //     
    //     return app;
    // }

    public static IHostApplicationBuilder AddRedis(this IHostApplicationBuilder app)
    {
        app.AddRedisDistributedCache("chatty-redis");

        return app;
    }
}