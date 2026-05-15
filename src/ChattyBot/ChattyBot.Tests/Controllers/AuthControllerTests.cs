using ChattyBot.Server.Api.Controllers;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ChattyBot.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly IAuthService _authService;
        private readonly AuthController _sut;

        public AuthControllerTests()
        {
            _authService = Substitute.For<IAuthService>();
            _sut = new AuthController(_authService);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenServiceSucceeds()
        {
            var dto = new RegisterDTO("test@email.com", "user", "Pass123!");
            var expectedResponse = new AuthResponseDTO { IsSuccess = true };
            _authService.RegisterAsync(dto).Returns(expectedResponse);

            var result = await _sut.Register(dto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedResponse);
            await _authService.Received(1).RegisterAsync(dto);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenServiceFails()
        {
            var dto = new RegisterDTO("existing@email.com", "user", "Pass123!");
            var expectedResponse = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "User already exists" };
            _authService.RegisterAsync(dto).Returns(expectedResponse);

            var result = await _sut.Register(dto);

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenServiceSucceeds()
        {
            var dto = new LoginDTO("user@email.com", "CorrectPass123!");
            var expectedResponse = new AuthResponseDTO { IsSuccess = true, Token = "fake-jwt-token" };
            _authService.LoginAsync(dto).Returns(expectedResponse);

            var result = await _sut.Login(dto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedResponse);
            await _authService.Received(1).LoginAsync(dto);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenServiceFails()
        {
            var dto = new LoginDTO("user@email.com", "WrongPass");
            var expectedResponse = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Invalid credentials" };
            _authService.LoginAsync(dto).Returns(expectedResponse);

            var result = await _sut.Login(dto);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be(expectedResponse);
        }
    }
}