using ChattyBot.Server.Application.BotEngine.Utils;
using FluentAssertions;

namespace ChattyBot.Tests.Utils
{
    public class CaesarCipherTests
    {
        [Theory]
        [InlineData("abc", 3, "def")]
        [InlineData("XYZ", 3, "ABC")] 
        [InlineData("Hello World!", 5, "Mjqqt Btwqi!")]
        [InlineData("123 @#$", 10, "123 @#$")]
        public void Encrypt_ShouldShiftLettersCorrectly(string input, int shift, string expected)
        {
            var result = CaesarCipher.Encrypt(input, shift);

            result.Should().Be(expected);
        }

        [Fact]
        public void Encrypt_ShouldHandleEmptyOrNullInput()
        {
            CaesarCipher.Encrypt("").Should().BeEmpty();
            CaesarCipher.Encrypt(null!).Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_ShouldMaintainCase()
        {
            string input = "AbC";

            var result = CaesarCipher.Encrypt(input, 1);

            result.Should().Be("BcD");
            char.IsUpper(result[0]).Should().BeTrue();
            char.IsLower(result[1]).Should().BeTrue();
        }

        [Fact]
        public void Encrypt_ShouldHandleLargeShifts_UsingModulo()
        {
            string input = "abc";

            var result = CaesarCipher.Encrypt(input, 26);

            result.Should().Be("abc");
        }
    }
}