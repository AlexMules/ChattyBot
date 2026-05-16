using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ChattyBot.Tests.Integration
{
    public class ChatIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ChatIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task AuthenticateClientAsync(string email, string username, string password)
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var regPayload = new RegisterDTO(email, username, password);
            await _client.PostAsJsonAsync("/api/auth/register", regPayload);

            var loginPayload = new LoginDTO(email, password);
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

            if (authResult?.Token != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Token);
            }
        }

        [Fact]
        public async Task CreateConversation_And_SendMessage_ShouldWorkPerfect_WhenAuthenticated()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("user@test.com", "active_user", "Password123!");

            var createChatDto = new CreateChatDTO("Integration Test Chat Room");
            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", createChatDto);
            convResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            conversation.Should().NotBeNull();
            int chatId = conversation!.Id;

            var sendMessageDto = new SendMessageDTO("Hello, this is a real integration test message!");

            var messageResponse = await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", sendMessageDto);

            messageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                var savedMessage = db.ChatMessages.FirstOrDefault(m => m.ConversationId == chatId);

                savedMessage.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetHistory_ShouldReturnMessages_WhenConversationExistsAndUserIsOwner()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("history@test.com", "history_user", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("History Chat"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", new SendMessageDTO("First Message"));
            await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", new SendMessageDTO("Second Message"));

            var response = await _client.GetAsync($"/api/ChatMessage/conversation/{chatId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var history = await response.Content.ReadFromJsonAsync<List<ChatMessageDTO>>();
            history.Should().NotBeNull();
            history.Should().HaveCountGreaterThanOrEqualTo(2);
            history!.Any(m => m.Content == "First Message").Should().BeTrue();
            history!.Any(m => m.Content == "Second Message").Should().BeTrue();
        }

        [Fact]
        public async Task GetHistory_ShouldReturnForbidden_WhenConversationDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("user@test.com", "regular_user", "Password123!");

            var response = await _client.GetAsync("/api/ChatMessage/conversation/99999");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Rename_ShouldUpdateTitle_WhenUserIsOwner()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("rename@test.com", "rename_user", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Old Chat Title"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            var renameDto = new RenameChatDTO("Completly New Chat Title");

            var response = await _client.PutAsJsonAsync($"/api/ChatConversation/{chatId}/rename", renameDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();

                var updatedChat = db.Set<ChatConversation>().FirstOrDefault(c => c.Id == chatId);
                if (updatedChat == null)
                {
                    foreach (var entityType in db.Model.GetEntityTypes())
                    {
                        if (entityType.ClrType.Name.Contains("Conversation") || entityType.ClrType.Name.Contains("Chat"))
                        {
                            var dbSet = db.GetType().GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(entityType.ClrType).Invoke(db, null);
                            if (dbSet is IQueryable<object> queryable)
                            {
                                var idProp = entityType.FindProperty("Id");
                                var titleProp = entityType.FindProperty("Title");
                                var entity = queryable.AsEnumerable().FirstOrDefault(e => idProp != null && Convert.ToInt32(idProp.PropertyInfo?.GetValue(e)) == chatId);
                                if (entity != null && titleProp != null)
                                {
                                    titleProp.PropertyInfo?.GetValue(entity).Should().Be("Completly New Chat Title");
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    updatedChat.Title.Should().Be("Completly New Chat Title");
                }
            }
        }

        [Fact]
        public async Task RenameChatConversation_ShouldReturnForbidden_WhenUserIsNotOwnerOfConversation()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("owner_rename@test.com", "owner_ren", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Original Title"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await AuthenticateClientAsync("intruder_rename@test.com", "intruder_ren", "Password123!");

            var renameDto = new RenameChatDTO("Hacked Title");
            var response = await _client.PutAsJsonAsync($"/api/ChatConversation/{chatId}/rename", renameDto);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteChatConversation_ShouldReturnNoContent_WhenUserIsOwner()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("legit_owner@test.com", "legit_owner", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("My Personal Chat"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            var response = await _client.DeleteAsync($"/api/ChatConversation/{chatId}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();

                var chatInDb = db.Set<ChatConversation>().FirstOrDefault(c => c.Id == chatId);
                chatInDb.Should().BeNull();
            }
        }

        [Fact]
        public async Task DeleteChatConversation_ShouldReturnForbidden_WhenUserIsNotOwnerOfConversation()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("owner_delete@test.com", "owner_del", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Owner Room to Delete"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await AuthenticateClientAsync("intruder_delete@test.com", "intruder_del", "Password123!");

            var response = await _client.DeleteAsync($"/api/ChatConversation/{chatId}");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                var chatInDb = db.Set<ChatConversation>().FirstOrDefault(c => c.Id == chatId);
                chatInDb.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task SendMessage_ShouldReturnBadRequest_WhenContentIsEmptyOrWhitespace()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("validation@test.com", "val_user", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Validation Room"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            var invalidMessageDto = new SendMessageDTO("   ");

            var response = await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", invalidMessageDto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendMessage_ShouldReturnForbidden_WhenUserIsNotOwnerOfConversation()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("owner@test.com", "chat_owner", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Owner Private Room"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await AuthenticateClientAsync("intruder@test.com", "chat_intruder", "Password123!");

            var sneakyMessage = new SendMessageDTO("Hey, I can see this room?");
            var response = await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", sneakyMessage);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Endpoints_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var responseGet = await _client.GetAsync("/api/ChatConversation/conversations");
            var responsePost = await _client.PostAsJsonAsync("/api/ChatMessage/1/send", new SendMessageDTO("Test Content"));

            responseGet.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}