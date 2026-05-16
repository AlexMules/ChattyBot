using ChattyBot.Client.Components;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using Bunit;

namespace ChattyBot.Tests.Integration
{
    public class ResiliencyTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ResiliencyTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public void BubbleComponent_ShouldHandleExceptionAndFallback_WhenJsonIsMalformed()
        {
            using var ctx = new BunitContext();
            string malformedJson = "{ malformed: json [ data }";

            var cut = ctx.Render<MusicBubble>(parameters => parameters
                .Add(p => p.RawContent, malformedJson));

            cut.Should().NotBeNull();
            cut.Markup.Should().NotBeNull();
        }

        [Fact]
        public async Task ProtectedEndpoint_ShouldReturnUnauthorized_WhenTokenIsExpired()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "expired_or_invalid_jwt_token_value");

            var response = await _client.GetAsync("/api/ChatConversation/conversations");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}