using ChattyBot.Server.Domain.Entities;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface IJokeRepository : IRandomRepository<Joke>
    {
    }
}
