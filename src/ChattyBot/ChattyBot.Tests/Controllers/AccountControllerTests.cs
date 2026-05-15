using ChattyBot.Server.Api.Controllers;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;

namespace ChattyBot.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly IAccountManagerService _accountService;
        private readonly AccountController _sut;
        private const int TestUserId = 123;

        public AccountControllerTests()
        {
            _accountService = Substitute.For<IAccountManagerService>();
            _sut = new AccountController(_accountService);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, TestUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenServiceSucceeds()
        {
            var dto = new ChangePasswordDTO("OldPass123!", "NewStrongPass123!");
            var serviceResult = new AuthResponseDTO { IsSuccess = true };

            _accountService.ChangePasswordAsync(TestUserId, dto).Returns(serviceResult);

            var result = await _sut.ChangePassword(dto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(serviceResult);
            await _accountService.Received(1).ChangePasswordAsync(TestUserId, dto);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenServiceFails()
        {
            var dto = new ChangePasswordDTO("WrongPass", "NewStrongPass123!");
            var serviceResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Invalid current password" };

            _accountService.ChangePasswordAsync(TestUserId, dto).Returns(serviceResult);

            var result = await _sut.ChangePassword(dto);

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(serviceResult);
        }

        [Fact]
        public async Task ChangeUsername_ShouldReturnOk_WhenServiceSucceeds()
        {
            var dto = new ChangeUsernameDTO("new_username");
            var serviceResult = new AuthResponseDTO { IsSuccess = true };

            _accountService.ChangeUsernameAsync(TestUserId, dto).Returns(serviceResult);

            var result = await _sut.ChangeUsername(dto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(serviceResult);
        }

        [Fact]
        public async Task ChangeUsername_ShouldReturnBadRequest_WhenServiceFails()
        {
            var dto = new ChangeUsernameDTO("taken_username");
            var serviceResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Username already exists" };

            _accountService.ChangeUsernameAsync(TestUserId, dto).Returns(serviceResult);

            var result = await _sut.ChangeUsername(dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ChangeEmail_ShouldReturnOk_WhenServiceSucceeds()
        {
            var dto = new ChangeEmailDTO("password123!", "new@email.com");
            var serviceResult = new AuthResponseDTO { IsSuccess = true };

            _accountService.ChangeEmailAsync(TestUserId, dto).Returns(serviceResult);

            var result = await _sut.ChangeEmail(dto);

            result.Should().BeOfType<OkObjectResult>();
            await _accountService.Received(1).ChangeEmailAsync(TestUserId, dto);
        }

        [Fact]
        public async Task ChangeEmail_ShouldReturnBadRequest_WhenServiceFails()
        {
            var dto = new ChangeEmailDTO("password123!", "existing@email.com");
            var serviceResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Email already in use" };

            _accountService.ChangeEmailAsync(TestUserId, dto).Returns(serviceResult);

            var result = await _sut.ChangeEmail(dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}