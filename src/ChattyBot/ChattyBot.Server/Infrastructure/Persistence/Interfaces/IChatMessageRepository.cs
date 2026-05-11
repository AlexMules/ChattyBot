using ChattyBot.Server.Domain.Entities;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<List<ChatMessage>> GetChatMessagesByConversationIdAsync(int conversationId);
        Task<ChatMessage> AddChatMessageAsync(ChatMessage message);
        Task DeleteAllChatMessagesInConversationAsync(int conversationId);
        Task<ChatMessage?> GetByIdAsync(int id);
        Task UpdateAsync(ChatMessage message);
    }
}
