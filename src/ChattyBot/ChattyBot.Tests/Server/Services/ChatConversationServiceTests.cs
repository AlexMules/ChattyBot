using ChattyBot.Server.Application.Services;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Server.Services
{
    public class ChatConversationServiceTests
    {
        private readonly IChatConversationRepository _repo;
        private readonly ChatConversationService _sut;

        public ChatConversationServiceTests()
        {
            _repo = Substitute.For<IChatConversationRepository>();
            _sut = new ChatConversationService(_repo);
        }

        [Fact]
        public async Task GetChatConversationsByUserIdAsync_ShouldReturnMappedDtos()
        {
            int userId = 1;
            var conversations = new List<ChatConversation>
            {
                new ChatConversation { Id = 1, Title = "Chat 1", CreatedAt = DateTime.UtcNow },
                new ChatConversation { Id = 2, Title = "Chat 2", CreatedAt = DateTime.UtcNow }
            };
            _repo.GetAllConversationsByUserIdAsync(userId).Returns(conversations);

            var result = await _sut.GetChatConversationsByUserIdAsync(userId);

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Chat 1");
            result[1].Id.Should().Be(2);
        }

        [Fact]
        public async Task CreateChatConversationAsync_ShouldAddBotWelcomeMessage()
        {
            int userId = 1;
            string username = "Alex";
            var dto = new CreateChatDTO("New Adventure");

            _repo.AddConversationAsync(Arg.Any<ChatConversation>())
                 .Returns(x => (ChatConversation)x[0]);

            var result = await _sut.CreateChatConversationAsync(userId, username, dto);

            result.Title.Should().Be("New Adventure");

            await _repo.Received(1).AddConversationAsync(Arg.Is<ChatConversation>(c =>
                c.UserId == userId &&
                c.Messages.Count == 1 &&
                c.Messages.First().Sender == MessageSender.Bot
            ));
        }

        [Fact]
        public async Task CreateChatConversationAsync_ShouldSetRecentTimestamp()
        {
            var dto = new CreateChatDTO("Title");
            _repo.AddConversationAsync(Arg.Any<ChatConversation>())
                 .Returns(x => (ChatConversation)x[0]);

            var result = await _sut.CreateChatConversationAsync(1, "User", dto);

            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task RenameConversationAsync_ShouldReturnNull_WhenConversationDoesNotExist()
        {
            _repo.GetChatConversationByIdAsync(Arg.Any<int>()).Returns((ChatConversation?)null);

            var result = await _sut.RenameConversationAsync(1, 999, "New Title");

            result.Should().BeNull();
        }

        [Fact]
        public async Task RenameConversationAsync_ShouldReturnFalse_WhenUserIsNotOwner()
        {
            var conversation = new ChatConversation { Id = 10, UserId = 99 };
            _repo.GetChatConversationByIdAsync(10).Returns(conversation);

            var result = await _sut.RenameConversationAsync(1, 10, "Hack Title");

            result.Should().BeFalse();
            await _repo.DidNotReceiveWithAnyArgs().UpdateTitleAsync(default, default!);
        }

        [Fact]
        public async Task RenameConversationAsync_ShouldReturnTrue_WhenSuccess()
        {
            var conversation = new ChatConversation { Id = 10, UserId = 1 };
            _repo.GetChatConversationByIdAsync(10).Returns(conversation);
            _repo.UpdateTitleAsync(10, "New Title").Returns(true);

            var result = await _sut.RenameConversationAsync(1, 10, "New Title");

            result.Should().BeTrue();
            await _repo.Received(1).UpdateTitleAsync(10, "New Title");
        }

        [Fact]
        public async Task RenameConversationAsync_ShouldReturnFalse_WhenRepositoryFailsToUpdate()
        {
            var conversation = new ChatConversation { Id = 1, UserId = 1 };
            _repo.GetChatConversationByIdAsync(1).Returns(conversation);
            _repo.UpdateTitleAsync(1, "New Title").Returns(false);

            var result = await _sut.RenameConversationAsync(1, 1, "New Title");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetConversationForExportAsync_ShouldReturnOrderedMessages()
        {
            var now = DateTime.UtcNow;
            var conversation = new ChatConversation
            {
                Title = "Export Chat",
                CreatedAt = now,
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Sender = MessageSender.User, Content = "Second", Timestamp = now.AddMinutes(5) },
                    new ChatMessage { Sender = MessageSender.Bot, Content = "First", Timestamp = now }
                }
            };
            _repo.GetConversationWithMessagesAsync(1).Returns(conversation);

            var result = await _sut.GetConversationForExportAsync(1);

            result.Should().NotBeNull();
            result!.Messages.Should().HaveCount(2);
            result.Messages.First().Content.Should().Be("First");
            result.Messages.Last().Content.Should().Be("Second");
        }

        [Fact]
        public async Task GetConversationForExportAsync_ShouldReturnNull_WhenNotFound()
        {
            _repo.GetConversationWithMessagesAsync(1).Returns((ChatConversation?)null);
            var result = await _sut.GetConversationForExportAsync(1);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteChatConversationAsync_ShouldCallRepo()
        {
            await _sut.DeleteChatConversationAsync(123);
            await _repo.Received(1).DeleteConversationAsync(123);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task IsUserOwnerAsync_ShouldReturnRepoResult(bool expected)
        {
            _repo.IsUserOwnerAsync(1, 1).Returns(expected);
            var result = await _sut.IsUserOwnerAsync(1, 1);
            result.Should().Be(expected);
        }
    }
}