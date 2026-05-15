using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using ChattyBot.Client.Services.State;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Client.State
{
    public class CustomAuthStateProviderTests
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly CustomAuthStateProvider _sut;

        public CustomAuthStateProviderTests()
        {
            _localStorage = Substitute.For<ILocalStorageService>();
            _httpClient = new HttpClient(); 
            _sut = new CustomAuthStateProvider(_localStorage, _httpClient);
        }

        private string CreateFakeJwt(Dictionary<string, object> claims)
        {
            var json = JsonSerializer.Serialize(claims);
            var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
                                       .Split('=')[0]; 
            return $"header.{payloadBase64}.signature";
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ValidToken_ReturnsAuthenticatedAndSetsHeader()
        {
            var futureExp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
            var token = CreateFakeJwt(new Dictionary<string, object>
            {
                { "exp", futureExp },
                { ClaimTypes.Name, "UserName" }
            });

            _localStorage.GetItemAsync<string>("authToken").Returns(token);

            var state = await _sut.GetAuthenticationStateAsync();

            state.User.Identity!.IsAuthenticated.Should().BeTrue();
            state.User.FindFirst(ClaimTypes.Name)!.Value.Should().Be("UserName");
            _httpClient.DefaultRequestHeaders.Authorization.Should().NotBeNull();
            _httpClient.DefaultRequestHeaders.Authorization!.Parameter.Should().Be(token);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_NoToken_ReturnsAnonymous()
        {
            _localStorage.GetItemAsync<string>("authToken").Returns((string)null!);

            var state = await _sut.GetAuthenticationStateAsync();

            state.User.Identity!.IsAuthenticated.Should().BeFalse();
            _httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ExpiredToken_ReturnsAnonymousAndCleansStorage()
        {
            var pastExp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds();
            var token = CreateFakeJwt(new Dictionary<string, object> { { "exp", pastExp } });

            _localStorage.GetItemAsync<string>("authToken").Returns(token);

            var state = await _sut.GetAuthenticationStateAsync();

            state.User.Identity!.IsAuthenticated.Should().BeFalse();
            await _localStorage.Received(1).RemoveItemAsync("authToken");
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_TokenMissingExp_ReturnsAnonymous()
        {
            var token = CreateFakeJwt(new Dictionary<string, object> { { "name", "User" } });
            _localStorage.GetItemAsync<string>("authToken").Returns(token);

            var state = await _sut.GetAuthenticationStateAsync();

            state.User.Identity!.IsAuthenticated.Should().BeFalse();
            await _localStorage.Received(1).RemoveItemAsync("authToken");
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_MalformedToken_TriggersCatchAndReturnsAnonymous()
        {
            _localStorage.GetItemAsync<string>("authToken").Returns("this-is-not-a-jwt");

            var state = await _sut.GetAuthenticationStateAsync();

            state.User.Identity!.IsAuthenticated.Should().BeFalse();
            await _localStorage.Received(1).RemoveItemAsync("authToken");
        }

        [Theory]
        [InlineData("{\"a\":\"b\"}")]       
        [InlineData("{\"user\":\"alex\"}")] 
        public async Task GetAuthenticationStateAsync_HandlesBase64PaddingCorrectly(string jsonContent)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonContent);
            var payloadNoPadding = Convert.ToBase64String(bytes).Split('=')[0];
            var token = $"header.{payloadNoPadding}.signature";

            _localStorage.GetItemAsync<string>("authToken").Returns(token);

            var act = async () => await _sut.GetAuthenticationStateAsync();

            await act.Should().NotThrowAsync<FormatException>();
        }

        [Fact]
        public async Task NotifyUserAuthentication_SavesToStorageAndNotifiesUI()
        {
            var token = CreateFakeJwt(new Dictionary<string, object> { { "sub", "123" } });
            bool eventRaised = false;
            _sut.AuthenticationStateChanged += (task) => eventRaised = true;

            await _sut.NotifyUserAuthentication(token);

            await _localStorage.Received(1).SetItemAsync("authToken", token);
            eventRaised.Should().BeTrue();
        }

        [Fact]
        public async Task NotifyUserLogout_ClearsStorageAndNotifiesUI()
        {
            bool eventRaised = false;
            _sut.AuthenticationStateChanged += (task) => eventRaised = true;

            await _sut.NotifyUserLogout();

            await _localStorage.Received(1).RemoveItemAsync("authToken");
            eventRaised.Should().BeTrue();

            var state = await _sut.GetAuthenticationStateAsync();
            state.User.Identity!.IsAuthenticated.Should().BeFalse();
        }
    }
}