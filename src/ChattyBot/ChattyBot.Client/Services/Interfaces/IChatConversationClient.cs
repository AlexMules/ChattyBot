using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Client.Services.Interfaces
{
    public interface IChatConversationClient
    {
        Task<List<ChatConversationDTO>> GetConversationsAsync();
        Task<ChatConversationDTO?> CreateConversationAsync(CreateChatDTO dto);
        Task<bool> DeleteConversationAsync(int id);
        Task<bool> RenameConversationAsync(int id, RenameChatDTO dto);
        Task<HttpResponseMessage> ExportConversationAsync(int id, string format);

    }
}
