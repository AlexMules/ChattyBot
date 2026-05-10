using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;

        public ChatMessageService(IChatMessageRepository messageRepo, IChatConversationRepository conversationRepo)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
        }

        public async Task<List<ChatMessageDTO>> GetChatMessagesByConversationIdAsync(int userId, int chatId)
        {
            if (!await _conversationRepo.IsUserOwnerAsync(userId, chatId))
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation!");
            }

            var messages = await _messageRepo.GetChatMessagesByConversationIdAsync(chatId);

            return messages.Select(m => new ChatMessageDTO(
                m.Id,
                m.Content,
                m.Sender.ToString(),
                m.Timestamp)).ToList();
        }

        public async Task<List<ChatMessageDTO>> AddChatMessageAsync(int userId, int chatId, SendMessageDTO dto)
        {
            if (!await _conversationRepo.IsUserOwnerAsync(userId, chatId))
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation.");
            }

            var userMsg = new ChatMessage
            {
                ConversationId = chatId,
                Content = dto.Content,
                Sender = MessageSender.User,
                Timestamp = DateTime.UtcNow
            };
            var savedUser = await _messageRepo.AddChatMessageAsync(userMsg);

            // mockup response method, MUST REPLACE with real bot logic later
            var savedBot = await GenerateBotResponseAsync(chatId, dto.Content);

            return new List<ChatMessageDTO>
                {
                    new ChatMessageDTO(savedUser.Id, savedUser.Content, savedUser.Sender.ToString(), savedUser.Timestamp),
                    new ChatMessageDTO(savedBot.Id, savedBot.Content, savedBot.Sender.ToString(), savedBot.Timestamp)
                };
        }

        // mockup response method, MUST REPLACE with real bot logic later
        private async Task<ChatMessage> GenerateBotResponseAsync(int chatId, string userMessage)
        {
            string botText = userMessage.ToLower().Contains("joke")
                ? "Why don't scientists trust atoms? Because they make up everything!"
                : "Message received by ChattyBot!";

            var botMsg = new ChatMessage
            {
                ConversationId = chatId,
                Content = botText,
                Sender = MessageSender.Bot,
                Timestamp = DateTime.UtcNow.AddMilliseconds(500)
            };

            return await _messageRepo.AddChatMessageAsync(botMsg);
        }
    }
}