namespace ChattyBot.Shared.Contracts.DTO
{
    public class ExportMessageDTO
    {
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        public ExportMessageDTO() { }

        public ExportMessageDTO(string sender, string content, DateTime timestamp)
        {
            Sender = sender;
            Content = content;
            Timestamp = timestamp;
        }
    }
}
