using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class QuoteRepository : BaseRandomRepository<Quote>, IQuoteRepository
    {
        public QuoteRepository(ChattyBotDbContext context) : base(context) { }
    }
}
