using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// * Redis *
var redis = builder.AddRedis("chatty-redis");

// * RabbitMQ *
var rbAdmin = builder.AddParameter("rabbit-admin", secret: true);
var rbPassword = builder.AddParameter("rabbit-password", secret: true);
var rabbitMq = builder.AddRabbitMQ("chatty-rabbitmq", rbAdmin, rbPassword)
    .WithManagementPlugin();;

// * Postgres *
var admin = builder.AddParameter("postgres-admin", secret: true);
var password = builder.AddParameter("postgres-password", secret: true);

var postgresDbServer = builder.AddPostgres("chatty-postgres-server", admin, password)
    .WithDataVolume(isReadOnly: false);

var authDb = postgresDbServer.AddDatabase("chatty-auth-db");
var appDb = postgresDbServer.AddDatabase("chatty-app-db");

// * PgAdmin in development *
if (builder.Environment.IsDevelopment())
{
    postgresDbServer.WithPgAdmin(c => c.WithHostPort(5050).WaitFor(postgresDbServer));
}

// * Auth Server *
builder.AddProject<Projects.Chatty_Authentication_Api>("chatty-auth-server")
    .WithReference(authDb)
    .WaitFor(authDb);

// * App Server *
builder.AddProject<Projects.Chatty_WebApi>("chatty-app-server")
    .WithReference(appDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WaitFor(appDb)
    .WaitFor(rabbitMq)
    .WaitFor(redis);

builder.Build().Run();