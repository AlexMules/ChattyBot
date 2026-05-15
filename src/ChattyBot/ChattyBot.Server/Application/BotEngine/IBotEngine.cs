namespace ChattyBot.Server.Application.BotEngine
{
    public interface IBotEngine
    {
        Task<BotResponse> ResolveAndExecuteAsync(string input, string username);
    }
}
