using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChattyBot.Tests.Server.Security
{
    public class TokenGeneratorTests
    {
        private readonly IConfiguration _configuration;
        private readonly TokenGenerator _sut;
        private const string TestKey = "a_very_long_and_secure_secret_key_123!";
        private const string TestIssuer = "ChattyBotServer";
        private const string TestAudience = "ChattyBotClient";

        public TokenGeneratorTests()
        {
            _configuration = Substitute.For<IConfiguration>();

            _configuration["Jwt:Key"].Returns(TestKey);
            _configuration["Jwt:Issuer"].Returns(TestIssuer);
            _configuration["Jwt:Audience"].Returns(TestAudience);

            _sut = new TokenGenerator(_configuration);
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowException_WhenKeyIsMissing()
        {
            _configuration["Jwt:Key"].Returns((string)null!);

            var action = () => _sut.GenerateJwtToken(new User { Id = 1 });

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("JWT Key is missing!");
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidToken_WithAllUserClaims()
        {
            var user = new User
            {
                Id = 42,
                Username = "user",
                Email = "user@email.com",
                AvatarPath = "custom_profile_pic.png"
            };

            var tokenString = _sut.GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(tokenString);

            jsonToken.Issuer.Should().Be(TestIssuer);
            jsonToken.Audiences.Should().Contain(TestAudience);

            jsonToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be("42");
            jsonToken.Claims.First(c => c.Type == ClaimTypes.Name).Value.Should().Be("user");
            jsonToken.Claims.First(c => c.Type == ClaimTypes.Email).Value.Should().Be("user@email.com");
            jsonToken.Claims.First(c => c.Type == "avatar").Value.Should().Be("custom_profile_pic.png");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateJwtToken_ShouldUseDefaultAvatar_WhenAvatarPathIsInvalid(string? invalidAvatarPath)
        {
            var user = new User
            {
                Id = 1,
                Username = "UserWithNoAvatar",
                Email = "test@test.com",
                AvatarPath = invalidAvatarPath
            };

            var tokenString = _sut.GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(tokenString);

            var avatarClaim = jsonToken.Claims.First(c => c.Type == "avatar").Value;
            avatarClaim.Should().Be("avatar1.png");
        }

        [Fact]
        public void GenerateJwtToken_ShouldSetExpirationToOneDay()
        {
            var user = new User { Id = 1, Username = "Test", Email = "a@b.com" };

            var tokenString = _sut.GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(tokenString);

            jsonToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), precision: TimeSpan.FromMinutes(5));
        }
    }
}