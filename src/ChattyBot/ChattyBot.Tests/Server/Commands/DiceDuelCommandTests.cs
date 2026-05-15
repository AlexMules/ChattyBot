using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class DiceDuelCommandTests
    {
        private readonly DiceDuelCommand _sut;

        public DiceDuelCommandTests()
        {
            _sut = new DiceDuelCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/dice-duel");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnDiceDuelMessageType()
        {
            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.DiceDuel);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnValidJsonWithBothRollsInRange()
        {
            var result = await _sut.ExecuteAsync();

            var jsonDoc = JsonDocument.Parse(result.Content);
            var root = jsonDoc.RootElement;

            root.TryGetProperty("UserRoll", out var userProp).Should().BeTrue();
            userProp.GetInt32().Should().BeInRange(1, 6);

            root.TryGetProperty("BotRoll", out var botProp).Should().BeTrue();
            botProp.GetInt32().Should().BeInRange(1, 6);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndWorkNormally()
        {
            var result = await _sut.ExecuteAsync("challenge me!");

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.DiceDuel);
            result.Content.Should().Contain("UserRoll").And.Contain("BotRoll");
        }
    }
}