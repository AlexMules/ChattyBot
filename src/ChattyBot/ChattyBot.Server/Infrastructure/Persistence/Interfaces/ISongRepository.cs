using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface ISongRepository : ICategorizedRandomRepository<Song, MusicCategory> { }
}
