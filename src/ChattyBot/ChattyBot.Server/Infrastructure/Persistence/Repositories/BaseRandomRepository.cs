using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRandomRepository<T> : IRandomRepository<T> where T : class
    {
        protected readonly ChattyBotDbContext _context;

        protected BaseRandomRepository(ChattyBotDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetRandomAsync()
        {
            return await _context.Set<T>()
                .OrderBy(x => EF.Functions.Random())
                .FirstOrDefaultAsync();
        }
    }
}