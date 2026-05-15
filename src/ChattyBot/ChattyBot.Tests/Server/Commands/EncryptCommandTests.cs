using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;

namespace ChattyBot.Tests.Server.Commands
{
    public class EncryptCommandTests
    {
        private readonly EncryptCommand _sut;

        public EncryptCommandTests()
        {
            _sut = new EncryptCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/encrypt");
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

            result.Content.Should().Contain("Please provide some text to encrypt");
            result.Type.Should().Be(MessageType.Text);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEncryptedText_WhenInputIsValid()
        {
            string input = "abc";
            string expected = "def";

            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Be(expected);
            result.Type.Should().Be(MessageType.Text);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleUppercaseAndSpacesCorrectly()
        {
            string input = "A B";

            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Be("D E");
        }
    }
}