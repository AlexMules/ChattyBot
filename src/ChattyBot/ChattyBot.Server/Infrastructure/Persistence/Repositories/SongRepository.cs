using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class SongRepository : BaseCategorizedRepository<Song, MusicCategory>, ISongRepository
    {
        public SongRepository(ChattyBotDbContext context) : base(context) { }
    }
}
