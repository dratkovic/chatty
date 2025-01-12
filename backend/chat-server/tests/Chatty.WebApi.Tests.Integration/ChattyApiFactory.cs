using System.Data.Common;
using System.Net.Http.Headers;
using Chatty.Auth.Api.Tests.Integration;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Domain.Models;
using Chatty.Core.Infrastructure.Authentication.TokenGenerator;
using Chatty.Domain;
using Chatty.Infrastructure.Persistance;
using Chatty.webApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Chatty.WebApi.Tests.Integration;

public class ChattyApiFactory : WebApplicationFactory<IChattyApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _sqlContainer = new PostgreSqlBuilder().Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.11")
        .WithName("chatty-rabbitmq")
        .Build();

    private DbConnection _dbConnection = default!;

    public HttpClient HttpClient { get; private set; } = null!;

    public IAuthenticatedUser User1 { get; private set; } = null!;
    public IAuthenticatedUser User2 { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var rabbitMqConnectionString = _rabbitMqContainer.GetConnectionString();

        builder.ConfigureServices(services =>
        {
            // Replace RabbitMQ connection string with test container details
            services.Replace(ServiceDescriptor.Singleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var updatedConfig = new ConfigurationBuilder()
                    .AddConfiguration(configuration)
                    .AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>("ConnectionStrings:chatty-rabbitmq", rabbitMqConnectionString)
                    })
                    .Build();
                return updatedConfig;
            }));

            services.RemoveAll(typeof(DbContextOptions));

            // ** Tricky part **
            services.RemoveAll(typeof(IDbContextPool<ChattyDbContext>));
            services.RemoveAll(typeof(IScopedDbContextLease<ChattyDbContext>));
            // needed to figure this out.. Aspire pooling

            services.RemoveAll(typeof(DbContextOptions<ChattyDbContext>));

            // * Again Tricky part *
            services.AddSingleton(new DbContextOptionsBuilder<ChattyDbContext>()
                .UseNpgsql(_sqlContainer.GetConnectionString())
                .Options);

            services.AddDbContextPool<ChattyDbContext>(options =>
            {
                options.UseNpgsql(_sqlContainer.GetConnectionString());
            });
            // * ================ *
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("ConnectionStrings:chatty-rabbitmq", rabbitMqConnectionString)
            });
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders(); // Optional: Clear other log providers if needed
            logging.AddConsole(); // Optional: Add console logging
        });

        builder.UseEnvironment("Development");
    }

    public string GetTokenForUser(IAuthenticatedUser user)
    {
        using var scope = Services.CreateScope();
        var services = scope.ServiceProvider;

        var jwtTokenManipulator = services.GetRequiredService<IJwtTokenManipulator>();

        return jwtTokenManipulator.GenerateToken(user);
    }

    public void SetAuthorizationHeader(IAuthenticatedUser user)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenForUser(user));
    }

    public void ResetHttpClient()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        await SeedDatabase();

        HttpClient = CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
        await _sqlContainer.DisposeAsync();
    }

    private async Task SeedDatabase()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        User1 = UserFaker.GetAuthenticatedUser();
        User2 = UserFaker.GetAuthenticatedUser();

        var user1 = User.Create(User1.Email, User1.FirstName + " " + User1.LastName, User1.Id);
        var user2 = User.Create(User2.Email, User2.FirstName + " " + User2.LastName, User2.Id);

        dbContext.Set<User>().Add(user1.Value);
        dbContext.Set<User>().Add(user2.Value);

        await dbContext.SaveChangesAsync();
    }
}