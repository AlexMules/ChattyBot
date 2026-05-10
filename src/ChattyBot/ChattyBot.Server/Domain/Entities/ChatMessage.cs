using ChattyBot.Server.Domain.Enums;
using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public MessageSender Sender { get; set; }

        public int ConversationId { get; set; }
        public ChatConversation ChatConversation { get; set; } = null!;
    }
}
