using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Shared.Contracts.Enums;
using ChattyBot.Server.Application.BotEngine;

namespace ChattyBot.Server.Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IBotEngine _botEngine;

        public ChatMessageService(
            IChatMessageRepository messageRepo,
            IChatConversationRepository conversationRepo,
            IBotEngine botEngine)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _botEngine = botEngine;
        }

        public async Task<List<ChatMessageDTO>> GetChatMessagesByConversationIdAsync(int userId, int chatId)
        {
            await EnsureUserOwnsConversationAsync(userId, chatId);

            var messages = await _messageRepo.GetChatMessagesByConversationIdAsync(chatId);

            return messages.Select(m => new ChatMessageDTO(
                m.Id,
                m.Content,
                m.Type,
                m.Sender.ToString(),
                m.Timestamp
            )).ToList();
        }

        public async Task<List<ChatMessageDTO>> AddChatMessageAsync(int userId, int chatId, SendMessageDTO dto, string username)
        {
            await EnsureUserOwnsConversationAsync(userId, chatId);

            var userMessageDto = await CreateSaveAndMapMessageAsync(
                chatId,
                dto.Content,
                MessageSender.User,
                MessageType.Text);

            var botResponse = await _botEngine.ResolveAndExecuteAsync(dto.Content, username);

            var botMessageDto = await CreateSaveAndMapMessageAsync(
                chatId,
                botResponse.Content.Trim(),
                MessageSender.Bot,
                botResponse.Type
            );

            return new List<ChatMessageDTO> { userMessageDto, botMessageDto };
        }

        private async Task EnsureUserOwnsConversationAsync(int userId, int chatId)
        {
            if (!await _conversationRepo.IsUserOwnerAsync(userId, chatId))
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation.");
            }
        }

        private async Task<ChatMessageDTO> CreateSaveAndMapMessageAsync(
            int chatId,
            string content,
            MessageSender sender,
            MessageType type)
        {
            var message = new ChatMessage
            {
                ConversationId = chatId,
                Content = content,
                Sender = sender,
                Timestamp = DateTime.UtcNow,
                Type = type     
            };

            var savedMessage = await _messageRepo.AddChatMessageAsync(message);

            return new ChatMessageDTO(
                savedMessage.Id,
                savedMessage.Content,
                savedMessage.Type,
                savedMessage.Sender.ToString(),
                savedMessage.Timestamp
            );
        }
    }
}