using ChattyBot.Server.Domain.Enums;

namespace ChattyBot.Server.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public MessageSender Sender { get; set; }

        public int ChatConversationId { get; set; }
        public ChatConversation ChatConversation { get; set; } = null!;
    }
}
