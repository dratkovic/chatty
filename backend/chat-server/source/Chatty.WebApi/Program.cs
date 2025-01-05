using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Infrastructure.LiveChat;
using Chatty.webApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddChattyApi();
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Ensure database is created.
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}

app.UseIEndpoints<IChattyApiMarker>();
app.MapHub<LiveChatHub>("hubs/livechat");

app.Run();

