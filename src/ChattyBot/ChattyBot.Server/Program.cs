using ChattyBot.Server.Application.BotEngine;
using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Application.Services;
using ChattyBot.Server.Commands;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Server.Infrastructure.Persistence.Repositories;
using ChattyBot.Server.Infrastructure.Security;
using ChattyBot.Server.Infrastructure.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found!");

builder.Services.AddDbContext<ChattyBotDbContext>(options =>
    options.UseMySQL(connectionString, x => x.MigrationsAssembly("ChattyBot.Server")));

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorLocalPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader(); 
    });
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatConversationRepository, ChatConversationRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IJokeRepository, JokeRepository>();
builder.Services.AddScoped<IFunFactRepository, FunFactRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IMemeRepository, MemeRepository>();
builder.Services.AddScoped<IVideoGameRepository, VideoGameRepository>();
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<ITriviaRepository, TriviaRepository>();

builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountManagerService, AccountManagerService>();
builder.Services.AddScoped<IChatConversationService, ChatConversationService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<ITriviaService, TriviaService>();

builder.Services.AddScoped<IBotCommand, HelpCommand>();
builder.Services.AddScoped<IBotCommand, JokeCommand>();
builder.Services.AddScoped<IBotCommand, FunFactCommand>();
builder.Services.AddScoped<IBotCommand, QuoteCommand>();
builder.Services.AddScoped<IBotCommand, MemeCommand>();
builder.Services.AddScoped<IBotCommand, VideoGameCommand>();
builder.Services.AddScoped<IBotCommand, MusicCommand>();
builder.Services.AddScoped<IBotCommand, TriviaCommand>();
builder.Services.AddScoped<IBotCommand, MorseCommand>();
builder.Services.AddScoped<IBotCommand, ReverseCommand>();
builder.Services.AddScoped<IBotCommand, EncryptCommand>();
builder.Services.AddScoped<IBotCommand, DiceCommand>();
builder.Services.AddScoped<IBotCommand, DiceDuelCommand>();
builder.Services.AddScoped<IBotCommand, CoinFlipCommand>();
builder.Services.AddScoped<IBotCommand, CalcCommand>();
builder.Services.AddScoped<IBotCommand, RandomCommand>();

builder.Services.AddScoped<BotEngine>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("BlazorLocalPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();