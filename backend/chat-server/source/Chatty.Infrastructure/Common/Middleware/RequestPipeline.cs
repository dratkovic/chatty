using Microsoft.AspNetCore.Builder;

namespace Chatty.Infrastructure.Common.Middleware;

public static class RequestPipeline
{
    public static IApplicationBuilder AddInfrastructureMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<EventualConsistencyMiddleware>();

        return builder;
    }
}