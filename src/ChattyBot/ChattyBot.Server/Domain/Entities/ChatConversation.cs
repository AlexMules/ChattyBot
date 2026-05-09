namespace ChattyBot.Server.Domain.Entities
{
    public class ChatConversation
    {
        public int Id { get; set; }
        public string Title { get; set; } = "New Chat";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<ChatMessage> Messages { get; set; } = new();
    }
}
