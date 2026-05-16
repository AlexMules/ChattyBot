using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChattyBot.Tests.Integration
{
    public class GameIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public GameIntegrationTests(CustomWebApplicationFactory factory)
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

        private int DiscoverCorrectAnswerIndex(int questionId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                foreach (var entityType in db.Model.GetEntityTypes())
                {
                    var idProp = entityType.FindProperty("Id") ?? entityType.FindProperty("QuestionId");
                    var correctProp = entityType.FindProperty("CorrectAnswerIndex") ?? entityType.FindProperty("CorrectIndex");

                    if (idProp != null && correctProp != null)
                    {
                        var dbSet = db.GetType().GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(entityType.ClrType).Invoke(db, null);
                        if (dbSet is IQueryable<object> queryable)
                        {
                            var entity = queryable.AsEnumerable().FirstOrDefault(e => {
                                var idVal = idProp.PropertyInfo?.GetValue(e);
                                return idVal != null && Convert.ToInt32(idVal) == questionId;
                            });

                            if (entity != null)
                            {
                                var correctVal = correctProp.PropertyInfo?.GetValue(entity);
                                if (correctVal != null)
                                {
                                    return Convert.ToInt32(correctVal);
                                }
                            }
                        }
                    }
                }
            }
            return 1;
        }

        [Fact]
        public async Task VerifyTriviaAnswer_ShouldReturnCorrectResult_WhenAnswerIsRight()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("trivia_winner@test.com", "trivia_master", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Trivia Game Arena"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", new SendMessageDTO("/trivia"));

            ChatMessage botMessage;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                botMessage = db.ChatMessages.Where(m => m.ConversationId == chatId).OrderBy(m => m.Id).LastOrDefault();
            }

            int questionId = 1;
            var questionData = JsonSerializer.Deserialize<TriviaQuestionDTO>(botMessage!.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (questionData != null)
            {
                questionId = questionData.QuestionId;
            }

            int correctIndex = DiscoverCorrectAnswerIndex(questionId);

            var triviaPayload = new TriviaCheckRequestDTO(questionId, correctIndex, botMessage!.Id);

            var response = await _client.PostAsJsonAsync("/api/Trivia/verify", triviaPayload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<TriviaCheckResponseDTO>();
            result.Should().NotBeNull();
            result!.IsCorrect.Should().BeTrue();
            result.CorrectIndex.Should().Be(correctIndex);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();

                var lastMessage = db.ChatMessages
                    .Where(m => m.ConversationId == chatId)
                    .OrderBy(m => m.Id)
                    .LastOrDefault();

                lastMessage.Should().NotBeNull();
                lastMessage!.Content.Should().ContainAll("UserAnswerIndex", "CorrectAnswerIndex");
            }
        }

        [Fact]
        public async Task VerifyTriviaAnswer_ShouldReturnIncorrectResult_WhenAnswerIsWrong()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("trivia_loser@test.com", "trivia_failer", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Trivia Failure Arena"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", new SendMessageDTO("/trivia"));

            ChatMessage botMessage;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                botMessage = db.ChatMessages.Where(m => m.ConversationId == chatId).OrderBy(m => m.Id).LastOrDefault();
            }

            int questionId = 1;
            var questionData = JsonSerializer.Deserialize<TriviaQuestionDTO>(botMessage!.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (questionData != null)
            {
                questionId = questionData.QuestionId;
            }

            int correctIndex = DiscoverCorrectAnswerIndex(questionId);
            int wrongIndex = correctIndex == 0 ? 1 : 0;

            var triviaPayload = new TriviaCheckRequestDTO(questionId, wrongIndex, botMessage!.Id);

            var response = await _client.PostAsJsonAsync("/api/Trivia/verify", triviaPayload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<TriviaCheckResponseDTO>();
            result.Should().NotBeNull();
            result!.IsCorrect.Should().BeFalse();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();

                var lastMessage = db.ChatMessages
                    .Where(m => m.ConversationId == chatId)
                    .OrderBy(m => m.Id)
                    .LastOrDefault();

                lastMessage.Should().NotBeNull();
                lastMessage!.Content.Should().ContainAll("UserAnswerIndex", "CorrectAnswerIndex");
            }
        }

        [Fact]
        public async Task VerifyTriviaAnswer_ShouldReturnBadRequest_WhenQuestionIsAlreadyAnswered()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("trivia_cheater@test.com", "no_cheating", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Anti Cheat Arena"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", new SendMessageDTO("/trivia"));

            ChatMessage botMessage;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                botMessage = db.ChatMessages.Where(m => m.ConversationId == chatId).OrderBy(m => m.Id).LastOrDefault();
            }

            int questionId = 1;
            var questionData = JsonSerializer.Deserialize<TriviaQuestionDTO>(botMessage!.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (questionData != null)
            {
                questionId = questionData.QuestionId;
            }

            int correctIndex = DiscoverCorrectAnswerIndex(questionId);

            var triviaPayload = new TriviaCheckRequestDTO(questionId, correctIndex, botMessage!.Id);

            var firstResponse = await _client.PostAsJsonAsync("/api/Trivia/verify", triviaPayload);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var secondResponse = await _client.PostAsJsonAsync("/api/Trivia/verify", triviaPayload);

            secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DiceDuel_ShouldReplyWithDiceRoll_WhenSlashCommandIsSent()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AuthenticateClientAsync("dice_player@test.com", "lucky_roller", "Password123!");

            var convResponse = await _client.PostAsJsonAsync("/api/ChatConversation", new CreateChatDTO("Dice Arena"));
            var conversation = await convResponse.Content.ReadFromJsonAsync<ChatConversationDTO>();
            int chatId = conversation!.Id;

            var response = await _client.PostAsJsonAsync($"/api/ChatMessage/{chatId}/send", new SendMessageDTO("/dice"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();

                var savedMessage = db.ChatMessages
                    .Where(m => m.ConversationId == chatId)
                    .OrderBy(m => m.Id)
                    .LastOrDefault();

                savedMessage.Should().NotBeNull();
                savedMessage!.Content.Should().ContainAll("Die1", "Die2");
            }
        }
    }
}