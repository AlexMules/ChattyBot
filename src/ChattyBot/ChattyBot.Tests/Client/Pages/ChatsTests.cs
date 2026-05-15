using Bunit;
using ChattyBot.Client.Pages;
using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using System.Security.Claims;
using System.Text.Json;

namespace ChattyBot.Client.Tests.Pages
{
    public class ChatTests : BunitContext
    {
        private readonly IChatConversationClient _convClientMock;
        private readonly IChatMessageClient _msgClientMock;
        private readonly ITriviaClient _triviaClientMock;
        private readonly AuthenticationStateProvider _authProviderMock;
        private readonly NavigationManager _navManager;

        public ChatTests()
        {
            _convClientMock = Substitute.For<IChatConversationClient>();
            _msgClientMock = Substitute.For<IChatMessageClient>();
            _triviaClientMock = Substitute.For<ITriviaClient>();
            _authProviderMock = Substitute.For<AuthenticationStateProvider>();

            Services.AddSingleton(_convClientMock);
            Services.AddSingleton(_msgClientMock);
            Services.AddSingleton(_triviaClientMock);
            Services.AddSingleton(_authProviderMock);

            _navManager = Services.GetRequiredService<NavigationManager>();
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private void SetupAuthState()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim("avatar", "avatar2.png")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _authProviderMock.GetAuthenticationStateAsync()
                .Returns(Task.FromResult(new AuthenticationState(user)));
        }

        private List<ChatConversationDTO> GetDummyConversations()
        {
            return new List<ChatConversationDTO>
            {
                new ChatConversationDTO(1, "First Chat", DateTime.Now),
                new ChatConversationDTO(2, "Second Chat", DateTime.Now.AddMinutes(-10))
            };
        }

        [Fact]
        public void OnInitialized_ShouldLoadConversationsAndHideLoadingState()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));

            var cut = Render<Chats>();

            cut.WaitForAssertion(() =>
            {
                var chatItems = cut.FindAll(".chat-title-text");
                chatItems.Count.Should().Be(2);
                chatItems[0].TextContent.Should().Be("First Chat");
                chatItems[1].TextContent.Should().Be("Second Chat");
            });
        }

        [Fact]
        public void SearchInput_ShouldFilterConversationsList()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));

            var cut = Render<Chats>();

            cut.WaitForElement(".chat-title-text");

            cut.Find(".search-input").Input("Second");

            var visibleChats = cut.FindAll(".chat-title-text");
            visibleChats.Count.Should().Be(1);
            visibleChats[0].TextContent.Should().Be("Second Chat");
        }

        [Fact]
        public void SidebarToggle_ShouldAddAndRemoveHiddenClass()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(new List<ChatConversationDTO>()));

            var cut = Render<Chats>();

            cut.Find("aside.sidebar").ClassList.Should().NotContain("hidden");
            cut.Find(".hamburger-btn").Click();
            cut.Find("aside.sidebar").ClassList.Should().Contain("hidden");
        }

        [Fact]
        public async Task CreateNewChat_ShouldCallApiAndSelectNewChat()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(new List<ChatConversationDTO>()));

            ChatConversationDTO newChatResult = new ChatConversationDTO(3, "My New Chat", DateTime.Now);
            _convClientMock.CreateConversationAsync(Arg.Any<CreateChatDTO>()).Returns(Task.FromResult(newChatResult));
            _msgClientMock.GetChatHistoryAsync(3).Returns(Task.FromResult(new List<ChatMessageDTO>()));

            var cut = Render<Chats>();
            cut.WaitForState(() => cut.FindAll(".chat-item.skeleton-item").Count == 0);

            cut.Find(".new-chat-btn").Click();

            cut.Find(".custom-modal input").Change("My New Chat");
            await cut.Find(".custom-modal .btn-confirm").ClickAsync();

            cut.WaitForAssertion(() =>
            {
                cut.FindAll(".chat-title-text").Any(e => e.TextContent == "My New Chat").Should().BeTrue();
                cut.Find(".current-chat-title").TextContent.Should().Be("My New Chat");
            });
        }

        [Fact]
        public async Task SelectChat_ShouldLoadHistoryAndDisplayMessages()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));

            var dummyMessages = new List<ChatMessageDTO>
            {
                new ChatMessageDTO(1, "Hello Bot", MessageType.Text, "User", DateTime.Now),
                new ChatMessageDTO(2, "Hello User", MessageType.Text, "Bot", DateTime.Now)
            };

            _msgClientMock.GetChatHistoryAsync(1).Returns(Task.FromResult(dummyMessages));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");

            await cut.FindAll(".chat-item")[0].ClickAsync();

            cut.WaitForAssertion(() =>
            {
                var textContents = cut.FindAll(".text-content");
                textContents.Count.Should().Be(2);
                textContents[0].TextContent.Should().Be("Hello Bot");
                textContents[1].TextContent.Should().Be("Hello User");
            });
        }

        [Fact]
        public async Task SelectChat_ShouldBlockInput_WhenLastMessageIsUnansweredTrivia()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));

            var triviaContent = JsonSerializer.Serialize(new TriviaQuestionDTO(1, "Dummy Question", new List<string>(), null, null));
            var dummyMessages = new List<ChatMessageDTO>
            {
                new ChatMessageDTO(1, triviaContent, MessageType.Trivia, "Bot", DateTime.Now)
            };

            _msgClientMock.GetChatHistoryAsync(1).Returns(Task.FromResult(dummyMessages));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");

            await cut.FindAll(".chat-item")[0].ClickAsync();

            cut.WaitForAssertion(() =>
            {
                cut.Find(".chat-footer input").HasAttribute("disabled").Should().BeTrue();
                cut.Find(".chat-footer input").GetAttribute("placeholder").Should().Be("Please answer the trivia question...");
            });
        }

        [Fact]
        public async Task SendMessage_ShouldCallApiUpdateListAndClearInput_UsingEnterKey()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));
            _msgClientMock.GetChatHistoryAsync(1).Returns(Task.FromResult(new List<ChatMessageDTO>()));

            var sentMessages = new List<ChatMessageDTO>
            {
                new ChatMessageDTO(3, "Test message", MessageType.Text, "User", DateTime.Now)
            };
            _msgClientMock.SendMessageAsync(1, Arg.Any<SendMessageDTO>()).Returns(Task.FromResult(sentMessages));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");
            await cut.FindAll(".chat-item")[0].ClickAsync();

            var input = cut.Find(".chat-footer input");

            input.Input("Test message");

            await input.KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            cut.WaitForAssertion(() =>
            {
                cut.FindAll(".text-content").Last().TextContent.Should().Be("Test message");
                cut.Find(".chat-footer input").GetAttribute("value").Should().BeNullOrEmpty();
            });
        }

        [Fact]
        public async Task RenameChat_ShouldCallApiAndReflectChangesInList_UsingEnterKey()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));
            _convClientMock.RenameConversationAsync(1, Arg.Any<RenameChatDTO>()).Returns(Task.FromResult(true));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");

            await cut.FindAll(".chat-menu-btn")[0].ClickAsync();
            await cut.FindAll(".menu-opt")[0].ClickAsync();

            var input = cut.Find(".custom-modal input");

            input.Change("Renamed Chat");
            await input.KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            cut.WaitForAssertion(() =>
            {
                cut.FindAll(".chat-title-text")[0].TextContent.Should().Be("Renamed Chat");
                cut.FindAll(".custom-modal").Should().BeEmpty();
            });
        }

        [Fact]
        public async Task DeleteChat_ShouldCallApiAndRemoveFromList()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));
            _convClientMock.DeleteConversationAsync(1).Returns(Task.FromResult(true));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");

            await cut.FindAll(".chat-menu-btn")[0].ClickAsync();
            await cut.Find(".delete-opt").ClickAsync();

            cut.WaitForAssertion(() =>
            {
                var remainingChats = cut.FindAll(".chat-title-text");
                remainingChats.Count.Should().Be(1);
                remainingChats[0].TextContent.Should().Be("Second Chat");
            });
        }

        [Fact]
        public async Task ExportChat_ShouldCallApiAndTriggerJsDownload()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));
            _msgClientMock.GetChatHistoryAsync(1).Returns(Task.FromResult(new List<ChatMessageDTO>()));

            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("fake-json-data")
            };
            _convClientMock.ExportConversationAsync(1, "json").Returns(Task.FromResult(httpResponse));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");
            await cut.FindAll(".chat-item")[0].ClickAsync();

            cut.Find(".export-header-btn").Click();
            await cut.FindAll(".custom-modal .btn-confirm")[0].ClickAsync();

            cut.WaitForAssertion(() =>
            {
                var invocations = JSInterop.Invocations.Select(i => i.Identifier);
                invocations.Should().Contain("downloadFileFromStream");
            });
        }

        [Fact]
        public async Task ExportChat_ShouldShowErrorToast_WhenApiThrowsException()
        {
            SetupAuthState();
            _convClientMock.GetConversationsAsync().Returns(Task.FromResult(GetDummyConversations()));
            _msgClientMock.GetChatHistoryAsync(1).Returns(Task.FromResult(new List<ChatMessageDTO>()));

            _convClientMock.ExportConversationAsync(1, "json").Returns(Task.FromException<HttpResponseMessage>(new Exception("Network error")));

            var cut = Render<Chats>();
            cut.WaitForElement(".chat-item");
            await cut.InvokeAsync(() => cut.FindAll(".chat-item")[0].Click());

            cut.Find(".export-header-btn").Click();

            var exportAction = cut.InvokeAsync(() => cut.FindAll(".custom-modal .btn-confirm")[0].Click());

            cut.WaitForAssertion(() =>
            {
                var toast = cut.Find(".toast-notification");
                toast.TextContent.Should().Contain("Export failed!");
            }, TimeSpan.FromSeconds(5));

            await exportAction;
        }
    }
}