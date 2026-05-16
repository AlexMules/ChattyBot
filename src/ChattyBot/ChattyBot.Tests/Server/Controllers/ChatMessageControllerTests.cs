using ChattyBot.Server.Api.Controllers;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Security.Claims;

namespace ChattyBot.Tests.Server.Controllers
{
    public class ChatMessageControllerTests
    {
        private readonly IChatMessageService _messageService;
        private readonly ChatMessageController _sut;
        private const int TestUserId = 1;
        private const string TestUsername = "UserName";

        public ChatMessageControllerTests()
        {
            _messageService = Substitute.For<IChatMessageService>();
            _sut = new ChatMessageController(_messageService);

            SetupUser(TestUserId, TestUsername);
        }

        private void SetupUser(int userId, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task GetHistory_ShouldReturnOk_WhenSuccessful()
        {
            int chatId = 10;
            var messages = new List<ChatMessageDTO>
            {
                new ChatMessageDTO(1, "Salut!", MessageType.Text, TestUsername, DateTime.Now)
            };
            _messageService.GetChatMessagesByConversationIdAsync(TestUserId, chatId).Returns(messages);

            var result = await _sut.GetHistory(chatId);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(messages);
        }

        [Fact]
        public async Task GetHistory_ShouldReturnUnauthorized_WhenUserIdIsZero()
        {
            SetupUser(0, "Guest");

            var result = await _sut.GetHistory(10);

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task GetHistory_ShouldReturnForbid_WhenServiceThrowsUnauthorizedAccess()
        {
            _messageService.GetChatMessagesByConversationIdAsync(TestUserId, 10)
                .Throws(new UnauthorizedAccessException("Not your chat!"));

            var result = await _sut.GetHistory(10);

            var actionResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            actionResult.StatusCode.Should().Be(403);
            actionResult.Value.Should().Be("Not your chat!");
        }

        [Fact]
        public async Task GetHistory_ShouldReturnForbidden_WhenConversationDoesNotExist()
        {
            _messageService.GetChatMessagesByConversationIdAsync(TestUserId, 99999)
                .Throws(new UnauthorizedAccessException("Conversation not found or access denied!"));

            var result = await _sut.GetHistory(99999);

            var actionResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            actionResult.StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task SendMessage_ShouldReturnOk_WithMessages()
        {
            int chatId = 10;
            var dto = new SendMessageDTO("Hello Bot");
            var response = new List<ChatMessageDTO>
            {
                new ChatMessageDTO(2, "Hello Bot", MessageType.Text, TestUsername, DateTime.Now)
            };

            _messageService.AddChatMessageAsync(TestUserId, chatId, dto, TestUsername).Returns(response);

            var result = await _sut.SendMessage(chatId, dto);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SendMessage_ShouldReturnBadRequest_WhenContentIsInvalid(string? invalidContent)
        {
            var dto = new SendMessageDTO(invalidContent!);

            var result = await _sut.SendMessage(10, dto);

            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Message content cannot be empty!");
        }

        [Fact]
        public async Task SendMessage_ShouldReturnForbid_WhenServiceThrowsException()
        {
            var dto = new SendMessageDTO("Valid content");
            _messageService.AddChatMessageAsync(TestUserId, 10, dto, TestUsername)
                .Throws(new UnauthorizedAccessException("Access denied!"));

            var result = await _sut.SendMessage(10, dto);

            var actionResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            actionResult.StatusCode.Should().Be(403);
        }
    }
}