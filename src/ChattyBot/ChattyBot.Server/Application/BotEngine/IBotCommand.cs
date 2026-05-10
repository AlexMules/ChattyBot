namespace ChattyBot.Server.Application.BotEngine
{
    public interface IBotCommand
    {
        string CommandTrigger { get; }
        Task<string> ExecuteAsync(string? parameters = null);
        string Description { get; }
    }
}
