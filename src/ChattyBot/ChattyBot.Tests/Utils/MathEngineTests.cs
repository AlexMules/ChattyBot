using ChattyBot.Server.Application.BotEngine.Utils;
using FluentAssertions;

namespace ChattyBot.Tests.Utils
{
    public class MathEngineTests
    {
        [Theory]
        [InlineData("2+2", "4")]
        [InlineData("10-5", "5")]
        [InlineData("3*4", "12")]
        [InlineData("20/4", "5")]
        [InlineData("2 + 2 * 3", "8")] 
        [InlineData("(2 + 2) * 3", "12")] 
        public void Compute_ShouldReturnCorrectResult_ForSimpleOperations(string expression, string expected)
        {
            var result = MathEngine.Compute(expression);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("10/3", "3.3333")] 
        [InlineData("1.25 + 1.25", "2.5")]
        [InlineData("0.1 + 0.2", "0.3")]
        public void Compute_ShouldHandleDecimalsAndRounding(string expression, string expected)
        {
            var result = MathEngine.Compute(expression);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("10/0", "DIV_ZERO")]
        [InlineData("0/0", "DIV_ZERO")]
        [InlineData("5 / (2-2)", "DIV_ZERO")]
        public void Compute_ShouldReturnDivZero_WhenDividingByZero(string expression, string expected)
        {
            var result = MathEngine.Compute(expression);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("abc + 1", "MATH_ERR")]
        [InlineData("", "MATH_ERR")]
        [InlineData(null, "MATH_ERR")]
        public void Compute_ShouldReturnMathErr_ForInvalidExpressions(string expression, string expected)
        {
            var result = MathEngine.Compute(expression);
            result.Should().Be(expected);
        }

        [Fact]
        public void Compute_ShouldUseInvariantCulture_ForDecimalPoints()
        {
            var result = MathEngine.Compute("5/2");
            result.Should().Be("2.5");
            result.Should().NotContain(",");
        }
    }
}