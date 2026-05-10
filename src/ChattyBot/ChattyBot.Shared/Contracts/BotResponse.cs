using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Shared.Contracts
{
    public record BotResponse(string Content, MessageType Type);
}
