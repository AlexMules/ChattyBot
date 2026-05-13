namespace ChattyBot.Shared.Contracts.DTO
{
    public record ExportConversationDTO(string Title,DateTime CreatedAt, List<ExportMessageDTO> Messages);
}
