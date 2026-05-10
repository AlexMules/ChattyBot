using ChattyBot.Server.Domain.Entities;

namespace ChattyBot.Server.Infrastructure.Persistence.Interfaces
{
    public interface IChatConversationRepository
    {
        Task<ChatConversation?> GetChatConversationByIdAsync(int id);
        Task<List<ChatConversation>> GetAllConversationsByUserIdAsync(int userId);
        Task<ChatConversation?> GetConversationByIdAsync(int id);
        Task<ChatConversation> AddConversationAsync(ChatConversation conversation);
        Task UpdateConversationAsync(ChatConversation conversation);
        Task DeleteConversationAsync(int id);
        Task<bool> IsUserOwnerAsync(int userId, int chatId);
        Task<bool> UpdateTitleAsync(int chatId, string newTitle);
    }
}
