using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class RpsCommandTests
    {
        private readonly RpsCommand _sut;

        public RpsCommandTests()
        {
            _sut = new RpsCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/rps");
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
            result.Content.Should().Contain("Choose your weapon");
        }

        [Theory]
        [InlineData("lizard")]
        [InlineData("spock")]
        [InlineData("gun")]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenChoiceIsInvalid(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("Invalid choice");
        }

        [Theory]
        [InlineData("-rock", "rock")]
        [InlineData("  -paper  ", "paper")]
        [InlineData("-SCISSORS", "scissors")]
        [InlineData("rock", "rock")]
        public async Task ExecuteAsync_ShouldNormalizeInput_AndReturnRpsDuelPayload(string input, string expectedUserChoice)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Type.Should().Be(MessageType.RpsDuel);

            var payload = JsonDocument.Parse(result.Content).RootElement;

            payload.GetProperty("UserChoice").GetString().Should().Be(expectedUserChoice);

            string botChoice = payload.GetProperty("BotChoice").GetString()!;
            new[] { "rock", "paper", "scissors" }.Should().Contain(botChoice);
        }
    }
}