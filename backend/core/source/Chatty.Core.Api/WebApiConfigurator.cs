namespace Chatty.Core.Api;

public static class WebApiConfigurator
{
    public static IServiceCollection AddCoreWebApi(this IServiceCollection services)
    {
        // We'll move CORS handling on rproxy side (nginx)
        services.AddCors(options =>
        {
            options.AddPolicy("ChattyCorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddHttpContextAccessor();
        
        return services;
    }
}