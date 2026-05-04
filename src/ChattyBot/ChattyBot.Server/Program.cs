using ChattyBot.Server.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found!");

builder.Services.AddDbContext<ChattyBotDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddDbContext<ChattyBotDbContext>(options =>
    options.UseMySQL(connectionString)); 

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();