using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class MemeRepository : BaseRandomRepository<Meme>, IMemeRepository
    {
        public MemeRepository(ChattyBotDbContext context) : base(context) { }
    }
}
