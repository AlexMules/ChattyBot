using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;

namespace ChattyBot.Tests.Commands
{
    public class ReverseCommandTests
    {
        private readonly ReverseCommand _sut;

        public ReverseCommandTests()
        {
            _sut = new ReverseCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/reverse");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ExecuteAsync_ShouldReturnUsageMessage_WhenParametersAreMissing(string? input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("Please provide some text to reverse");
        }

        [Theory]
        [InlineData("hello", "olleh")]
        [InlineData("12345", "54321")]
        [InlineData("A B C", "C B A")]
        public async Task ExecuteAsync_ShouldReturnReversedText_WhenInputIsValid(string input, string expected)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Be(expected);
        }
    }
}