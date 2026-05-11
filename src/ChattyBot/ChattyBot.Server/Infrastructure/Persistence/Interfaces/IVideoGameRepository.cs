using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

public interface IVideoGameRepository : IRandomRepository<VideoGame>
{
    Task<VideoGame?> GetRandomByCategoryAsync(GameCategory category);
}