using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Application.BotEngine;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Server.Commands
{
    public class AboutCommandTests
    {
        private readonly BotCommandContext _context;
        private readonly AboutCommand _sut;

        public AboutCommandTests()
        {
            _context = Substitute.For<BotCommandContext>();
            _sut = new AboutCommand(_context);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/about");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnWelcomeMessage_WithUsernameFromContext()
        {
            string testUser = "Alex";
            _context.Username.Returns(testUser);

            var result = await _sut.ExecuteAsync();

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.Text);

            result.Content.Should().Contain(testUser);
            result.Content.Should().Contain("ChattyBot");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndStillWork()
        {
            _context.Username.Returns("Guest");

            var result = await _sut.ExecuteAsync("some random parameters");

            result.Should().NotBeNull();
            result.Content.Should().NotBeNullOrEmpty();
            result.Content.Should().Contain("Guest");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleNullParametersGracefully()
        {
            _context.Username.Returns("User");

            var result = await _sut.ExecuteAsync(null);

            result.Should().NotBeNull();
            result.Content.Should().Contain("User");
        }
    }
}