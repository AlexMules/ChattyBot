using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class DiceCommandTests
    {
        private readonly DiceCommand _sut;

        public DiceCommandTests()
        {
            _sut = new DiceCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/dice");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnDiceMessageType()
        {
            var result = await _sut.ExecuteAsync();
            result.Type.Should().Be(MessageType.Dice);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnValidJsonWithDiceValuesInRange()
        {
            var result = await _sut.ExecuteAsync();

            var jsonDoc = JsonDocument.Parse(result.Content);
            var root = jsonDoc.RootElement;

            root.TryGetProperty("Die1", out var die1Prop).Should().BeTrue();
            die1Prop.GetInt32().Should().BeInRange(1, 6);

            root.TryGetProperty("Die2", out var die2Prop).Should().BeTrue();
            die2Prop.GetInt32().Should().BeInRange(1, 6);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndWorkNormally()
        {
            var result = await _sut.ExecuteAsync("roll it!");

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.Dice);
            result.Content.Should().Contain("Die1");
            result.Content.Should().Contain("Die2");
        }
    }
}