using ChattyBot.Server.Api.Controllers;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Security.Claims;

namespace ChattyBot.Tests.Controllers
{
    public class ChatConversationControllerTests
    {
        private readonly IChatConversationService _conversationService;
        private readonly IExportService _exportService;
        private readonly ChatConversationController _sut;
        private const int TestUserId = 1;
        private const string TestUsername = "UserName";

        public ChatConversationControllerTests()
        {
            _conversationService = Substitute.For<IChatConversationService>();
            _exportService = Substitute.For<IExportService>();
            _sut = new ChatConversationController(_conversationService, _exportService);

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

        [Theory]
        [InlineData("Get")]
        [InlineData("Create")]
        [InlineData("Delete")]
        [InlineData("Export")]
        public async Task Methods_ShouldReturnUnauthorized_WhenUserIdIsZero(string method)
        {
            SetupUser(0, "Guest");
            IActionResult result;

            result = method switch
            {
                "Get" => (await _sut.GetUserConversations()).Result!,
                "Create" => (await _sut.CreateChatConversation(new CreateChatDTO("Title"))).Result!,
                "Delete" => await _sut.DeleteChatConversation(10),
                "Export" => await _sut.Export(10),
                _ => throw new ArgumentException()
            };

            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task GetUserConversations_ShouldReturnOk_WithList()
        {
            var conversations = new List<ChatConversationDTO> { new(10, "Test", DateTime.Now) };
            _conversationService.GetChatConversationsByUserIdAsync(TestUserId).Returns(conversations);

            var result = await _sut.GetUserConversations();

            result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(conversations);
        }

        [Fact]
        public async Task CreateChatConversation_ShouldReturnOk_WithNewChat()
        {
            var dto = new CreateChatDTO("New Chat");
            var createdChat = new ChatConversationDTO(1, "New Chat", DateTime.Now);
            _conversationService.CreateChatConversationAsync(TestUserId, TestUsername, dto).Returns(createdChat);

            var result = await _sut.CreateChatConversation(dto);

            result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(createdChat);
        }

        [Fact]
        public async Task DeleteChatConversation_ShouldReturnNoContent_WhenSuccessful()
        {
            _conversationService.IsUserOwnerAsync(TestUserId, 10).Returns(true);
            _conversationService.DeleteChatConversationAsync(10).Returns(true);

            var result = await _sut.DeleteChatConversation(10);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteChatConversation_ShouldReturnForbid_WhenUserIsNotOwner()
        {
            _conversationService.IsUserOwnerAsync(TestUserId, 10).Returns(false);

            var result = await _sut.DeleteChatConversation(10);

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task DeleteChatConversation_ShouldReturnNotFound_WhenServiceReturnsFalse()
        {
            _conversationService.IsUserOwnerAsync(TestUserId, 10).Returns(true);
            _conversationService.DeleteChatConversationAsync(10).Returns(false);

            var result = await _sut.DeleteChatConversation(10);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Rename_ShouldReturnOk_WhenResultIsTrue()
        {
            _conversationService.RenameConversationAsync(TestUserId, 10, "New").Returns(true);
            var result = await _sut.Rename(10, new RenameChatDTO("New"));
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Rename_ShouldReturnForbid_WhenResultIsFalse()
        {
            _conversationService.RenameConversationAsync(TestUserId, 10, "New").Returns(false);
            var result = await _sut.Rename(10, new RenameChatDTO("New"));
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Rename_ShouldReturnNotFound_WhenResultIsNull()
        {
            _conversationService.RenameConversationAsync(TestUserId, 10, "New").Returns((bool?)null);
            var result = await _sut.Rename(10, new RenameChatDTO("New"));
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Export_ShouldReturnFile_WhenSuccessful()
        {
            var mockData = Substitute.For<ExportConversationDTO>();
            var mockFile = new ExportedFileDTO(new byte[] { 1, 2 }, "application/json", "chat.json");

            _conversationService.IsUserOwnerAsync(TestUserId, 10).Returns(true);
            _conversationService.GetConversationForExportAsync(10).Returns(mockData);
            _exportService.CreateExportFile(mockData, "json").Returns(mockFile);

            var result = await _sut.Export(10, "json");

            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.FileDownloadName.Should().Be("chat.json");
            fileResult.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task Export_ShouldReturnForbid_WhenUserIsNotOwner()
        {
            _conversationService.IsUserOwnerAsync(TestUserId, 10).Returns(false);

            var result = await _sut.Export(10);

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Export_ShouldReturnNotFound_WhenDataIsNull()
        {
            _conversationService.IsUserOwnerAsync(TestUserId, 10).Returns(true);
            _conversationService.GetConversationForExportAsync(10).ReturnsNull();

            var result = await _sut.Export(10);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}