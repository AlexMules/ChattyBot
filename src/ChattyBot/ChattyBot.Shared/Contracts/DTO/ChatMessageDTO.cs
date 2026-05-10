using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Shared.Contracts.DTO
{
    public record ChatMessageDTO(
        int Id,
        string Content,
        MessageType Type,
        string Sender,
        DateTime Timestamp);
}
