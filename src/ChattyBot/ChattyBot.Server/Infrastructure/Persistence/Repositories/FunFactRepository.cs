using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class FunFactRepository : BaseRandomRepository<FunFact>, IFunFactRepository
    {
        public FunFactRepository(ChattyBotDbContext context) : base(context) { }
    }
}
