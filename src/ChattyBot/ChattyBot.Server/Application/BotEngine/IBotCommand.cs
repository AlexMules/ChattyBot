using ChattyBot.Shared.Contracts;

namespace ChattyBot.Server.Application.BotEngine
{
    public interface IBotCommand
    {
        string CommandTrigger { get; }
        string Description { get; }
        Task<BotResponse> ExecuteAsync(string? parameters = null);
    }
}
