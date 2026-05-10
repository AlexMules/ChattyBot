using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine
{
    public record BotResponse(string Content, MessageType Type);
}
