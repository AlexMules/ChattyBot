using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Commands
{
    public class CoinFlipCommandTests
    {
        private readonly CoinFlipCommand _sut;

        public CoinFlipCommandTests()
        {
            _sut = new CoinFlipCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/coinflip");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnCoinFlipMessageType()
        {
            var result = await _sut.ExecuteAsync();
            result.Type.Should().Be(MessageType.CoinFlip);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnValidJsonWithHeadsOrTails()
        {
            var result = await _sut.ExecuteAsync();

            var jsonDoc = JsonDocument.Parse(result.Content);
            var root = jsonDoc.RootElement;

            root.TryGetProperty("Result", out var resultProp).Should().BeTrue();

            string value = resultProp.GetString()!;
            value.Should().Match(s => s == "Heads" || s == "Tails");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndWorkNormally()
        {
            var result = await _sut.ExecuteAsync("unnecessary text");

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.CoinFlip);
        }
    }
}