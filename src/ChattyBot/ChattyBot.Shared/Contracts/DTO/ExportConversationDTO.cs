namespace ChattyBot.Shared.Contracts.DTO
{
    public class ExportConversationDTO
    {
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ExportMessageDTO> Messages { get; set; } = new();

        public ExportConversationDTO() { }

        public ExportConversationDTO(string title, DateTime createdAt, List<ExportMessageDTO> messages)
        {
            Title = title;
            CreatedAt = createdAt;
            Messages = messages;
        }
    }
}
