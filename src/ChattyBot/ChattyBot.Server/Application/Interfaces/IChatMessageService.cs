using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Interfaces
{
    public interface IChatMessageService
    {
        Task<List<ChatMessageDTO>> GetChatMessagesByConversationIdAsync(int userId, int chatId);
        Task<List<ChatMessageDTO>> AddChatMessageAsync(int userId, int chatId, SendMessageDTO dto, string username);
    }
}
