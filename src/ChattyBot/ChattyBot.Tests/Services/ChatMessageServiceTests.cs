using ChattyBot.Server.Application.Services;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Shared.Contracts.Enums;
using ChattyBot.Server.Application.BotEngine;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Services
{
    public class ChatMessageServiceTests
    {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IBotEngine _botEngine;
        private readonly ChatMessageService _sut;

        public ChatMessageServiceTests()
        {
            _messageRepo = Substitute.For<IChatMessageRepository>();
            _conversationRepo = Substitute.For<IChatConversationRepository>();
            _botEngine = Substitute.For<IBotEngine>();

            _sut = new ChatMessageService(_messageRepo, _conversationRepo, _botEngine);
        }


        [Fact]
        public async Task GetChatMessages_ShouldThrowUnauthorized_WhenUserIsNotOwner()
        {
            _conversationRepo.IsUserOwnerAsync(1, 10).Returns(false);

            var act = () => _sut.GetChatMessagesByConversationIdAsync(1, 10);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GetChatMessages_ShouldReturnMappedDtos_WhenUserIsOwner()
        {
            int chatId = 10;
            _conversationRepo.IsUserOwnerAsync(1, chatId).Returns(true);

            var messages = new List<ChatMessage>
            {
                new ChatMessage { Id = 1, Content = "Msg 1", Sender = MessageSender.User, Type = MessageType.Text, Timestamp = DateTime.UtcNow }
            };
            _messageRepo.GetChatMessagesByConversationIdAsync(chatId).Returns(messages);

            var result = await _sut.GetChatMessagesByConversationIdAsync(1, chatId);

            result.Should().NotBeEmpty();
            result[0].Content.Should().Be("Msg 1");
            result[0].Sender.Should().Be("User");
        }

        [Fact]
        public async Task AddChatMessageAsync_ShouldThrowUnauthorized_WhenUserIsNotOwner()
        {
            _conversationRepo.IsUserOwnerAsync(1, 10).Returns(false);
            var dto = new SendMessageDTO("Hack attempt");

            var act = () => _sut.AddChatMessageAsync(1, 10, dto, "User");

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            await _messageRepo.DidNotReceive().AddChatMessageAsync(Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task AddChatMessageAsync_ShouldProcessFlowCorrectly_AndReturnMappedMessages()
        {
            int userId = 1;
            int chatId = 10;
            string username = "Alex";
            var dto = new SendMessageDTO("/joke");

            _conversationRepo.IsUserOwnerAsync(userId, chatId).Returns(true);

            var botResponse = new BotResponse("   Why did the programmer quit? Because he didn't get arrays.   ", MessageType.Text);
            _botEngine.ResolveAndExecuteAsync(dto.Content, username).Returns(botResponse);

            _messageRepo.AddChatMessageAsync(Arg.Any<ChatMessage>()).Returns(x =>
            {
                var m = (ChatMessage)x[0];
                m.Id = new Random().Next(1, 1000);
                return m;
            });

            var result = await _sut.AddChatMessageAsync(userId, chatId, dto, username);

            result.Should().HaveCount(2);

            result[0].Content.Should().Be("/joke");
            result[0].Sender.Should().Be("User");

            result[1].Content.Should().Be("Why did the programmer quit? Because he didn't get arrays.");
            result[1].Sender.Should().Be("Bot");
            result[1].Type.Should().Be(MessageType.Text);

            result[0].Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            result[1].Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

            await _botEngine.Received(1).ResolveAndExecuteAsync("/joke", username);
            await _messageRepo.Received(2).AddChatMessageAsync(Arg.Is<ChatMessage>(m => m.ConversationId == chatId));
        }
    }
}