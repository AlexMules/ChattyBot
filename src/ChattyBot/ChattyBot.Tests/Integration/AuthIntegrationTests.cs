using ChattyBot.Server.Infrastructure.Persistence.Context;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ChattyBot.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldHashPassword_AndSaveUserToDatabase_WhenDataIsValid()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            var registerPayload = new RegisterDTO(
                email: "testuser@domain.com",
                username: "test_user",
                password: "Password123!"
            );

            var response = await _client.PostAsJsonAsync("/api/auth/register", registerPayload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                var savedUser = db.Users.FirstOrDefault(u => u.Email == "testuser@domain.com");

                savedUser.Should().NotBeNull();
                savedUser!.Username.Should().Be("test_user");
                savedUser.PasswordHash.Should().NotBe("Password123!");
                savedUser.PasswordHash.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            var firstPayload = new RegisterDTO("duplicate@domain.com", "original_user", "Password123!");
            await _client.PostAsJsonAsync("/api/auth/register", firstPayload);

            var duplicatePayload = new RegisterDTO("duplicate@domain.com", "other_user", "Password123!");

            var response = await _client.PostAsJsonAsync("/api/auth/register", duplicatePayload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ShouldReturnValidToken_WhenCredentialsAreCorrect()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            var registerPayload = new RegisterDTO("testuser@domain.com", "test_user", "Password123!");
            await _client.PostAsJsonAsync("/api/auth/register", registerPayload);

            var loginPayload = new LoginDTO(
                email: "testuser@domain.com",
                password: "Password123!"
            );

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
            authResult.Should().NotBeNull();
            authResult!.IsSuccess.Should().BeTrue();
            authResult.Token.Should().NotBeNullOrWhiteSpace();
            authResult.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            var registerPayload = new RegisterDTO("testuser@domain.com", "test_user", "Password123!");
            await _client.PostAsJsonAsync("/api/auth/register", registerPayload);

            var invalidLoginPayload = new LoginDTO(
                email: "testuser@domain.com",
                password: "WrongPassword123!"
            );

            var response = await _client.PostAsJsonAsync("/api/auth/login", invalidLoginPayload);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChattyBotDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            var loginPayload = new LoginDTO(
                email: "nonexistent@domain.com",
                password: "Password123!"
            );

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}