using System.Security.Claims;
using ChattyBot.Server.Infrastructure.Security;
using FluentAssertions;

namespace ChattyBot.Tests.Security
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetUserId_ShouldReturnIntegerId_WhenNameIdentifierClaimExists()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "123")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var result = principal.GetUserId();

            result.Should().Be(123);
        }

        [Fact]
        public void GetUserId_ShouldReturnZero_WhenNameIdentifierClaimIsMissing()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            var result = principal.GetUserId();

            result.Should().Be(0);
        }

        [Fact]
        public void GetUserId_ShouldThrowFormatException_WhenClaimValueIsNotANumber()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "not-a-number")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            Action action = () => principal.GetUserId();

            action.Should().Throw<FormatException>();
        }
    }
}