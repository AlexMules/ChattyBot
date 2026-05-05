using ChattyBot.Server.Domain.Entities;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task <User?> GetUserByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> SaveChangesAsync();

    }
}
