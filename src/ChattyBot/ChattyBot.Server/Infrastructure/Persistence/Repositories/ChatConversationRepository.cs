using Microsoft.EntityFrameworkCore;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class ChatConversationRepository : IChatConversationRepository
    {
        private readonly ChattyBotDbContext _context;

        public ChatConversationRepository(ChattyBotDbContext context) => _context = context;

        public async Task<ChatConversation?> GetChatConversationByIdAsync(int id)
        {
            return await _context.ChatConversations.FindAsync(id);
        }

        public async Task<List<ChatConversation>> GetAllConversationsByUserIdAsync(int userId)
        {
            return await _context.ChatConversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ChatConversation?> GetConversationByIdAsync(int id)
        {
            return await _context.ChatConversations.FindAsync(id);
        }

        public async Task<ChatConversation> AddConversationAsync(ChatConversation conversation)
        {
            _context.ChatConversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task UpdateConversationAsync(ChatConversation conversation)
        {
            _context.ChatConversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConversationAsync(int id)
        {
            var conv = await _context.ChatConversations.FindAsync(id);
            if (conv != null)
            {
                _context.ChatConversations.Remove(conv);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsUserOwnerAsync(int userId, int chatId)
        {
            return await _context.ChatConversations
                .AnyAsync(c => c.Id == chatId && c.UserId == userId);
        }

        public async Task<bool> UpdateTitleAsync(int chatId, string newTitle)
        {
            var conversation = await _context.ChatConversations.FindAsync(chatId);
            if (conversation == null)
            {
                return false;
            }

            conversation.Title = newTitle;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
