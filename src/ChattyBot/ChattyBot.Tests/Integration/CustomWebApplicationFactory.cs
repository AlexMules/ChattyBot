using ChattyBot.Server.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChattyBot.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ChattyBotDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions)).ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                var internalServiceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                var uniqueDbName = $"ChattyBotDb_{Guid.NewGuid()}";

                services.AddDbContext<ChattyBotDbContext>(options =>
                {
                    options.UseInMemoryDatabase(uniqueDbName)
                           .UseInternalServiceProvider(internalServiceProvider);
                });
            });
        }
    }
}