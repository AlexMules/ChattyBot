using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public abstract class BaseCategorizedRepository<T, TEnum> : BaseRandomRepository<T>, ICategorizedRandomRepository<T, TEnum>
        where T : class
        where TEnum : Enum
    {
        public BaseCategorizedRepository(ChattyBotDbContext context) : base(context) { }

        public async Task<T?> GetRandomByCategoryAsync(TEnum category)
        {
            return await _context.Set<T>()
                .Where(x => EF.Property<TEnum>(x, "Category").Equals(category))
                .OrderBy(x => EF.Functions.Random())
                .FirstOrDefaultAsync();
        }
    }
}