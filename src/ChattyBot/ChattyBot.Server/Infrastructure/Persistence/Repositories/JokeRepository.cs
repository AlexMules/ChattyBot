using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Server.Infrastructure.Persistence.Repositories;

public class JokeRepository : BaseRandomRepository<Joke>, IJokeRepository
{
    public JokeRepository(ChattyBotDbContext context) : base(context) { }
}