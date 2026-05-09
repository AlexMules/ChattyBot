using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Entities;
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

        public async Task<ChatConversationDTO> CreateChatConversationAsync(int userId, CreateChatDTO dto)
        {
            var entity = new ChatConversation
            {
                UserId = userId,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow
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
    }
}
