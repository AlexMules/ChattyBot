using Microsoft.EntityFrameworkCore;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Infrastructure.Persistence.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly ChattyBotDbContext _context;

        public ChatMessageRepository(ChattyBotDbContext context) => _context = context;

        public async Task<List<ChatMessage>> GetChatMessagesByConversationIdAsync(int conversationId)
        {
            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<ChatMessage> AddChatMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task DeleteAllChatMessagesInConversationAsync(int conversationId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .ToListAsync();

            _context.ChatMessages.RemoveRange(messages);
            await _context.SaveChangesAsync();
        }

        public async Task<ChatMessage?> GetByIdAsync(int id)
        {
            return await _context.ChatMessages.FindAsync(id);
        }

        public async Task UpdateAsync(ChatMessage message)
        {
            _context.ChatMessages.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}
