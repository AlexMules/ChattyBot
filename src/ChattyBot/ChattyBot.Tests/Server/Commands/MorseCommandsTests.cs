using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;

namespace ChattyBot.Tests.Server.Commands
{
    public class MorseCommandTests
    {
        private readonly MorseCommand _sut;

        public MorseCommandTests()
        {
            _sut = new MorseCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/morse");
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

            result.Content.Should().Contain("Please provide some text");
            result.Type.Should().Be(MessageType.Text);
        }

        [Theory]
        [InlineData("Hello!")] 
        [InlineData("abc#")]  
        [InlineData("2+2")]  
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenInputContainsInvalidChars(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("Error: Morse translation only supports");
            result.Type.Should().Be(MessageType.Text);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTranslatedText_WhenInputIsValid()
        {
            string input = "SOS";

            var result = await _sut.ExecuteAsync(input);

            result.Should().NotBeNull();
            result.Content.Should().NotBeNullOrEmpty();
            result.Content.Should().Contain(".").And.Contain("-");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleSpacesCorrectly()
        {
            var result = await _sut.ExecuteAsync("A B");

            result.Content.Should().NotBeNullOrEmpty();
            result.Content.Should().NotContain("Error");
        }
    }
}