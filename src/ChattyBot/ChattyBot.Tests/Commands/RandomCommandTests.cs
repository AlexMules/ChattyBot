using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;

namespace ChattyBot.Tests.Commands
{
    public class RandomCommandTests
    {
        private readonly RandomCommand _sut;

        public RandomCommandTests()
        {
            _sut = new RandomCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/random");
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
        public async Task ExecuteAsync_ShouldReturnUsageMessage_WhenParametersAreEmpty(string? input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("Provide Min and Max values");
            result.Type.Should().Be(MessageType.Text);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenOnlyOneParameterProvided()
        {
            var result = await _sut.ExecuteAsync("10");

            result.Content.Should().Contain("I need both a Min and a Max value");
        }

        [Theory]
        [InlineData("abc 10")]
        [InlineData("10 xyz")]
        [InlineData("a b")]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenParametersAreNotIntegers(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("Please use valid integers");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenMinIsGreaterThanMax()
        {
            var result = await _sut.ExecuteAsync("100 10");

            result.Content.Should().Contain("Invalid Range");
            result.Content.Should().Contain("cannot be greater than Max");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNumberWithinRange_WhenInputIsValid()
        {
            int min = 1;
            int max = 10;

            var result = await _sut.ExecuteAsync($"{min} {max}");

            int parsedResult = int.Parse(result.Content);
            parsedResult.Should().BeInRange(min, max);
            result.Type.Should().Be(MessageType.Text);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleNegativeNumbersCorrectly()
        {
            var result = await _sut.ExecuteAsync("-10 -5");

            int parsedResult = int.Parse(result.Content);
            parsedResult.Should().BeInRange(-10, -5);
        }
    }
}