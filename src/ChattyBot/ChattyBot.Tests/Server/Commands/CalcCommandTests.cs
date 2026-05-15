using ChattyBot.Server.Application.BotEngine.Commands;
using FluentAssertions;

namespace ChattyBot.Tests.Server.Commands
{
    public class CalcCommandTests
    {
        private readonly CalcCommand _sut;

        public CalcCommandTests()
        {
            _sut = new CalcCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/calc");
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
        public async Task ExecuteAsync_ShouldReturnUsageMessage_WhenParametersAreMissing(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("I need a math expression");
        }

        [Theory]
        [InlineData("2 + x")]
        [InlineData("10 $ 2")]
        [InlineData("2 @ 5")]
        public async Task ExecuteAsync_ShouldReturnInvalidCharsMessage_WhenInputContainsForbiddenChars(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("not a valid math expression");
        }

        [Theory]
        [InlineData("2 + + 2")]
        [InlineData("5 * / 2")]
        [InlineData("10 +")]
        [InlineData("* 5")]
        [InlineData("( )")]
        public async Task ExecuteAsync_ShouldReturnInvalidStructureMessage_WhenOperatorsAreMisused(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("invalid expression structure");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReplaceCommaWithDot_AndCalculateCorrectly()
        {
            var result = await _sut.ExecuteAsync("2,5 + 2,5");

            result.Content.Should().Be("Result: 5");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleDivisionByZero_FromEngine()
        {
            var result = await _sut.ExecuteAsync("10 / 0");

            result.Content.Should().Be("Error: Division by zero is not allowed!");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMathErrorMessage_OnInvalidEngineResult()
        {
            var result = await _sut.ExecuteAsync("(2 + 2");

            result.Content.Should().Contain("Error: That looks like an invalid expression!");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnCorrectResult_ForComplexValidExpression()
        {
            var result = await _sut.ExecuteAsync("(10 + 5) * 2 / 4");

            result.Content.Should().Be("Result: 7.5");
        }
    }
}