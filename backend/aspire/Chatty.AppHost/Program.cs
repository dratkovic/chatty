using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// * Redis *
var redis = builder.AddRedis("chatty-redis");

// * Postgres *
var admin = builder.AddParameter("postgres-admin", secret: true);
var password = builder.AddParameter("postgres-password", secret: true);

var postgresDbServer = builder.AddPostgres("chatty-postgres-server", admin, password)
    .WithDataVolume(isReadOnly: false);

var authDb = postgresDbServer.AddDatabase("chatty-auth-db");

// * PgAdmin in development *
if (builder.Environment.IsDevelopment())
{
    postgresDbServer.WithPgAdmin(c => c.WithHostPort(5050).WaitFor(postgresDbServer));
}

// * Auth Server *
builder.AddProject<Projects.Chatty_Authentication_Api>("chatty-auth-server")
    .WithReference(authDb)
    .WaitFor(authDb);

builder.Build().Run();