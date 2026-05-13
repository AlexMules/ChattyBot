using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class VideoGameRepository : BaseCategorizedRepository<VideoGame, GameCategory>, IVideoGameRepository
    {
        public VideoGameRepository(ChattyBotDbContext context) : base(context) { }
    }
}