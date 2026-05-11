using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface ITriviaRepository : ICategorizedRandomRepository<TriviaQuestion, TriviaCategory>
    {
        Task<TriviaQuestion?> GetByIdAsync(int id);
    }
}
