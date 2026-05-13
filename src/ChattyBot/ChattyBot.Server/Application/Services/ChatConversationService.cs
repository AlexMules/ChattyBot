using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Services
{
    public class ChatConversationService : IChatConversationService
    {
        private readonly IChatConversationRepository _repo;

        public ChatConversationService(IChatConversationRepository repo) => _repo = repo;

        public async Task<List<ChatConversationDTO>> GetChatConversationsByUserIdAsync(int userId)
        {
            var entities = await _repo.GetAllConversationsByUserIdAsync(userId);
            return entities.Select(c => new ChatConversationDTO(c.Id, c.Title, c.CreatedAt)).ToList();
        }

        public async Task<ChatConversationDTO> CreateChatConversationAsync(int userId, string username, CreateChatDTO dto)
        {
            var welcomeMessage = BotEngine.BotEngine.GetWelcomeMessage(username);
            var now = DateTime.UtcNow;

            ChatMessage welcomeChatMessage = new ChatMessage
            {
                Sender = MessageSender.Bot,
                Content = welcomeMessage.Content,
                Type = welcomeMessage.Type,
                Timestamp = now
            };

            var entity = new ChatConversation
            {
                UserId = userId,
                Title = dto.Title,
                CreatedAt = now,
                Messages = new List<ChatMessage> { welcomeChatMessage }
            };

            var saved = await _repo.AddConversationAsync(entity);

            return new ChatConversationDTO(saved.Id, saved.Title, saved.CreatedAt);
        }

        public async Task<bool> DeleteChatConversationAsync(int chatId)
        {
            await _repo.DeleteConversationAsync(chatId);
            return true;
        }

        public async Task<bool> IsUserOwnerAsync(int userId, int chatId)
        {
            return await _repo.IsUserOwnerAsync(userId, chatId);
        }

        public async Task<bool?> RenameConversationAsync(int userId, int chatId, string newTitle)
        {
            var conversation = await _repo.GetChatConversationByIdAsync(chatId);

            if (conversation == null)
            {
                return null;
            }
            if (conversation.UserId != userId)
            {
                return false;
            }

            var success = await _repo.UpdateTitleAsync(chatId, newTitle);
            return success;
        }

        public async Task<ExportConversationDTO?> GetConversationForExportAsync(int chatId)
        {
            var conversation = await _repo.GetConversationWithMessagesAsync(chatId);

            if (conversation == null)
            {
                return null;
            }

            return new ExportConversationDTO(
                conversation.Title,
                conversation.CreatedAt,
                conversation.Messages
                    .OrderBy(m => m.Timestamp)
                    .Select(m => new ExportMessageDTO(
                        m.Sender.ToString(),
                        m.Content,
                        m.Timestamp
                    )).ToList()
            );
        }
    }
}
