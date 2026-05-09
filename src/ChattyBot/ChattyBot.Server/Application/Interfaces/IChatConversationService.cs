using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Interfaces
{
    public interface IChatConversationService
    {
        Task<List<ChatConversationDTO>> GetChatConversationsByUserIdAsync(int userId);
        Task<ChatConversationDTO> CreateChatConversationAsync(int userId, CreateChatDTO dto);
        Task<bool> DeleteChatConversationAsync(int chatId);
        Task<bool> IsUserOwnerAsync(int userId, int chatId);
    }
}
