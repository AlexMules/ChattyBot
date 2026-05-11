namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface ICategorizedRandomRepository<T, TEnum> : IRandomRepository<T>
        where T : class
        where TEnum : Enum
    {
        Task<T?> GetRandomByCategoryAsync(TEnum category);
    }
}