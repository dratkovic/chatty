using System.Data.Common;
using Chatty.Authentication.Api;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Infrastructure;
using Chatty.Authentication.Api.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace Chatty.Auth.Api.Tests.Integration;

public class AuthApiFactory : WebApplicationFactory<IAuthenticationApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _sqlContainer = new PostgreSqlBuilder().Build();

    private DbConnection _dbConnection = default!;

    public HttpClient HttpClient { get; private set; } = null!;
    public IEmailSender EmailSender { get; } = 
        Substitute.For<IEmailSender>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll(typeof(IEmailSender));

            // ** Tricky part **
            services.RemoveAll(typeof(IDbContextPool<AuthenticationDbContext>));
            services.RemoveAll(typeof(IScopedDbContextLease<AuthenticationDbContext>));
            // needed to figure this out.. Aspire pooling

            services.RemoveAll(typeof(DbContextOptions<AuthenticationDbContext>));
            
            services.AddSingleton(_ => EmailSender);
            
            // * Again Tricky part *
            services.AddSingleton(new DbContextOptionsBuilder<AuthenticationDbContext>()
                .UseNpgsql(_sqlContainer.GetConnectionString())
                .Options);
            
            services.AddDbContextPool<AuthenticationDbContext>(options =>
            {
                options.UseNpgsql(_sqlContainer.GetConnectionString());
            });
            // * ================ *
        });
        
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders(); // Optional: Clear other log providers if needed
            logging.AddConsole(); // Optional: Add console logging
        });

        builder.UseEnvironment("Development");
    }

    public void ResetHttpClient()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;
    }
    
    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        //await SeedDatabase();

        HttpClient = CreateClient();
    }

    public Task DisposeAsync()
    {
        return _sqlContainer.DisposeAsync().AsTask();
    }

    private async Task SeedDatabase()
    {
        using var scope =Services.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

        await Seeder.Seed(dbContext, roleManager, userManager, true);
        var options = new DbContextOptionsBuilder<AuthenticationDbContext>()
            .UseNpgsql(_sqlContainer.GetConnectionString())
            .Options;

        await using var context = new AuthenticationDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }
}
