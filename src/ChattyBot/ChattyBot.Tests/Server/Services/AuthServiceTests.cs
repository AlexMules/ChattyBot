using ChattyBot.Server.Application.Services;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Server.Infrastructure.Security.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Server.Services
{
    public class AuthServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _tokenGenerator = Substitute.For<ITokenGenerator>();
            _sut = new AuthService(_userRepository, _tokenGenerator);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            var dto = new RegisterDTO("new@user.com", "NewUser", "StrongPass123!", "avatar2.png");
            _userRepository.GetByEmailAsync(dto.Email).Returns((User?)null);

            var result = await _sut.RegisterAsync(dto);

            result.IsSuccess.Should().BeTrue();

            await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
                u.Email == dto.Email &&
                u.Username == dto.Username &&
                u.AvatarPath == dto.AvatarPath && 
                !string.IsNullOrEmpty(u.PasswordHash) && 
                u.PasswordHash != dto.Password 
            ));

            await _userRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnError_WhenEmailIsAlreadyTaken()
        {
            var dto = new RegisterDTO("taken@user.com", "User", "password", "path");
            _userRepository.GetByEmailAsync(dto.Email).Returns(new User { Email = dto.Email });

            var result = await _sut.RegisterAsync(dto);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email is already taken!");
            await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var password = "password123";
            var email = "user@test.com";
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _userRepository.GetByEmailAsync(email).Returns(user);
            _tokenGenerator.GenerateJwtToken(user).Returns("mock-jwt-token");

            var result = await _sut.LoginAsync(new LoginDTO(email, password));

            result.IsSuccess.Should().BeTrue();
            result.Token.Should().Be("mock-jwt-token");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserDoesNotExist()
        {
            _userRepository.GetByEmailAsync("nonexistent@test.com").Returns((User?)null);

            var result = await _sut.LoginAsync(new LoginDTO("nonexistent@test.com", "any"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid email or password!");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenPasswordIsIncorrect()
        {
            var email = "user@test.com";
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password")
            };

            _userRepository.GetByEmailAsync(email).Returns(user);

            var result = await _sut.LoginAsync(new LoginDTO(email, "wrong_password"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid email or password!");
            _tokenGenerator.DidNotReceive().GenerateJwtToken(Arg.Any<User>());
        }
    }
}