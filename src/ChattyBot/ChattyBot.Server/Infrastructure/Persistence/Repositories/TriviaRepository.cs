using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class TriviaRepository : BaseCategorizedRepository<TriviaQuestion, TriviaCategory>, ITriviaRepository
    {
        public TriviaRepository(ChattyBotDbContext context) : base(context) { }

        public async Task<TriviaQuestion?> GetByIdAsync(int id)
        {
            return await _context.Set<TriviaQuestion>().FindAsync(id);
        }
    }
}
