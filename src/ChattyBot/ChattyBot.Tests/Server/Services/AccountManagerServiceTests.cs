using ChattyBot.Server.Application.Services;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Server.Infrastructure.Security.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Server.Services
{
    public class AccountManagerServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AccountManagerService _sut;

        public AccountManagerServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _tokenGenerator = Substitute.For<ITokenGenerator>();
            _sut = new AccountManagerService(_userRepository, _tokenGenerator);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnError_WhenUserNotFound()
        {
            _userRepository.GetUserByIdAsync(Arg.Any<int>()).Returns((User?)null);

            var result = await _sut.ChangePasswordAsync(1, new ChangePasswordDTO("old", "new"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnError_WhenCurrentPasswordIsWrong()
        {
            var user = new User { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password") };
            _userRepository.GetUserByIdAsync(1).Returns(user);

            var result = await _sut.ChangePasswordAsync(1, new ChangePasswordDTO("wrong_password", "new_password"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("The current password is wrong!");
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldSucceed_WhenDataIsValid()
        {
            var user = new User { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("old_password") };
            _userRepository.GetUserByIdAsync(1).Returns(user);

            var result = await _sut.ChangePasswordAsync(1, new ChangePasswordDTO("old_password", "new_password"));

            result.IsSuccess.Should().BeTrue();
            BCrypt.Net.BCrypt.Verify("new_password", user.PasswordHash).Should().BeTrue();
            await _userRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task ChangeUsernameAsync_ShouldReturnError_WhenUserNotFound()
        {
            _userRepository.GetUserByIdAsync(Arg.Any<int>()).Returns((User?)null);

            var result = await _sut.ChangeUsernameAsync(1, new ChangeUsernameDTO("NewUser"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");
        }

        [Fact]
        public async Task ChangeUsernameAsync_ShouldRollback_WhenTokenGenerationFails()
        {
            var oldUsername = "OldUser";
            var user = new User { Id = 1, Username = oldUsername };
            _userRepository.GetUserByIdAsync(1).Returns(user);

            _tokenGenerator.When(tg => tg.GenerateJwtToken(user))
                          .Do(x => { throw new Exception("JWT Fail"); });

            var result = await _sut.ChangeUsernameAsync(1, new ChangeUsernameDTO("NewUser"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("No changes were made!");
            user.Username.Should().Be(oldUsername);
        }

        [Fact]
        public async Task ChangeUsernameAsync_ShouldSucceed_AndGenerateTokenWithUpdatedData()
        {
            var oldUsername = "OldUser";
            var newUsername = "NewUser";
            var mockToken = "new_jwt_token";
            var user = new User { Id = 1, Username = oldUsername };

            _userRepository.GetUserByIdAsync(1).Returns(user);
            _tokenGenerator.GenerateJwtToken(Arg.Is<User>(u => u.Username == newUsername)).Returns(mockToken);

            var result = await _sut.ChangeUsernameAsync(1, new ChangeUsernameDTO(newUsername));

            result.IsSuccess.Should().BeTrue();
            result.Token.Should().Be(mockToken);
            user.Username.Should().Be(newUsername);
            _tokenGenerator.Received(1).GenerateJwtToken(Arg.Is<User>(u => u.Username == newUsername));
            await _userRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task ChangeEmailAsync_ShouldReturnError_WhenUserNotFound()
        {
            _userRepository.GetUserByIdAsync(Arg.Any<int>()).Returns((User?)null);

            var result = await _sut.ChangeEmailAsync(1, new ChangeEmailDTO("pass", "new@email.com"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");
        }

        [Fact]
        public async Task ChangeEmailAsync_ShouldReturnError_WhenPasswordIsIncorrect()
        {
            var userId = 1;
            var user = new User
            {
                Id = userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password")
            };
            _userRepository.GetUserByIdAsync(userId).Returns(user);

            var result = await _sut.ChangeEmailAsync(userId, new ChangeEmailDTO("wrong_password", "new@email.com"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Incorrect password!");
            await _userRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task ChangeEmailAsync_ShouldReturnError_WhenEmailIsAlreadyUsed()
        {
            var user = new User { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _userRepository.GetUserByIdAsync(1).Returns(user);
            _userRepository.GetByEmailAsync("taken@email.com").Returns(new User { Id = 2 });

            var result = await _sut.ChangeEmailAsync(1, new ChangeEmailDTO("password", "taken@email.com"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email address is already used!");
        }

        [Fact]
        public async Task ChangeEmailAsync_ShouldRollback_WhenTokenGenerationFails()
        {
            var oldEmail = "old@test.com";
            var user = new User { Id = 1, Email = oldEmail, PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _userRepository.GetUserByIdAsync(1).Returns(user);
            _userRepository.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);

            _tokenGenerator.When(tg => tg.GenerateJwtToken(user))
                          .Do(x => { throw new Exception("DB Error"); });

            var result = await _sut.ChangeEmailAsync(1, new ChangeEmailDTO("password", "new@test.com"));

            result.IsSuccess.Should().BeFalse();
            user.Email.Should().Be(oldEmail);
            result.ErrorMessage.Should().Contain("An internal error occurred!");
        }

        [Fact]
        public async Task ChangeEmailAsync_ShouldSucceed_AndGenerateTokenWithUpdatedData()
        {
            var oldEmail = "old@test.com";
            var newEmail = "new@test.com";
            var mockToken = "new_jwt_token";
            var user = new User { Id = 1, Email = oldEmail, PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

            _userRepository.GetUserByIdAsync(1).Returns(user);
            _userRepository.GetByEmailAsync(newEmail).Returns((User?)null);
            _tokenGenerator.GenerateJwtToken(Arg.Is<User>(u => u.Email == newEmail)).Returns(mockToken);

            var result = await _sut.ChangeEmailAsync(1, new ChangeEmailDTO("password", newEmail));

            result.IsSuccess.Should().BeTrue();
            result.Token.Should().Be(mockToken);
            user.Email.Should().Be(newEmail);
            _tokenGenerator.Received(1).GenerateJwtToken(Arg.Is<User>(u => u.Email == newEmail));
            await _userRepository.Received(1).SaveChangesAsync();
        }
    }
}