using Chatty.Core.Api;
using Chatty.Infrastructure;

namespace Chatty.webApi;

public static class ApiConfigurator
{
    public static IHostApplicationBuilder AddChattyApi(this IHostApplicationBuilder app)
    {
        app.AddInfrastructure();
        
        app.Services
            .AddCoreWebApi();
        
        return app;
    }
}