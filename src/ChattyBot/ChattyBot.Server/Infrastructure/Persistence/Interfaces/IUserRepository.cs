using ChattyBot.Server.Domain.Entities;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}
