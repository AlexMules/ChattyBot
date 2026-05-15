using ChattyBot.Client;
using ChattyBot.Client.Services.ApiClients;
using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Client.Services.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7025/")
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IAuthClient, AuthClient>();
builder.Services.AddScoped<IManageAccountClient, ManageAccountClient>();
builder.Services.AddScoped<IChatConversationClient, ChatConversationClient>();
builder.Services.AddScoped<IChatMessageClient, ChatMessageClient>();
builder.Services.AddScoped<ITriviaClient, TriviaClient>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped(sp => (CustomAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

await builder.Build().RunAsync();