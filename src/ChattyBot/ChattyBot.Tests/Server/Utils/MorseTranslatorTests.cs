using ChattyBot.Server.Application.BotEngine.Utils;
using FluentAssertions;
namespace ChattyBot.Tests.Server.Utils
{
    public class MorseTranslatorTests
    {
        [Theory]
        [InlineData("SOS", "... --- ...")]
        [InlineData("sos", "... --- ...")] 
        [InlineData("A B", ".- / -...")] 
        [InlineData("123", ".---- ..--- ...--")]
        public void ToMorse_ShouldTranslateValidInput_Correctly(string input, string expected)
        {
            var result = MorseTranslator.ToMorse(input);
            result.Should().Be(expected);
        }

        [Fact]
        public void ToMorse_ShouldReturnEmptyString_ForNullOrWhiteSpace()
        {
            MorseTranslator.ToMorse(null!).Should().BeEmpty();
            MorseTranslator.ToMorse("   ").Should().BeEmpty();
        }

        [Fact]
        public void ToMorse_ShouldIgnoreCharactersNotInDictionary_ButContinueProcessing()
        {
            var result = MorseTranslator.ToMorse("A! B");
            result.Should().Be(".- / -...");
        }
    }
}