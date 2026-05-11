using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public class VideoGameRepository : BaseRandomRepository<VideoGame>, IVideoGameRepository
{
    public VideoGameRepository(ChattyBotDbContext context) : base(context) { }

    public async Task<VideoGame?> GetRandomByCategoryAsync(GameCategory category)
    {
        return await _context.VideoGames
            .Where(g => g.Category == category)
            .OrderBy(x => EF.Functions.Random())
            .FirstOrDefaultAsync();
    }
}