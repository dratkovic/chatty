/*using Microsoft.EntityFrameworkCore;

namespace Sensei.Common.Infrastructure;

public static class MigrationExtensions
{
    public static void ApplyMigrations<T>(this IApplicationBuilder app) where T : DbContext 
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AuthenticationDbContext>();
        context.Database.Migrate();
    }
}*/