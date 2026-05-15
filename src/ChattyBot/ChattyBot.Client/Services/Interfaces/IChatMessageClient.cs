using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Client.Services.Interfaces
{
    public interface IChatMessageClient
    {
        Task<List<ChatMessageDTO>> GetChatHistoryAsync(int chatId);
        Task<List<ChatMessageDTO>> SendMessageAsync(int chatId, SendMessageDTO dto);

    }
}
