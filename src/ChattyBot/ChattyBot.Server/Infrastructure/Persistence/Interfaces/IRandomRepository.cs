namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface IRandomRepository<T> where T : class
    {
        Task<T?> GetRandomAsync();
    }
}