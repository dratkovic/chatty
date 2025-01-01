using Chatty.Authentication.Api;
using Chatty.Authentication.Api.Contracts;
using Chatty.Authentication.Api.Infrastructure;
using Chatty.Core.Api.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

builder.AddNpgsqlDbContext<AuthenticationDbContext>(ApiConstants.DbConnectionStringConfigName);

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("ChattyCorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIEndpoints<IAuthenticationApiMarker>();

app.Seed(app.Environment.IsDevelopment()).Wait();

app.Run();
