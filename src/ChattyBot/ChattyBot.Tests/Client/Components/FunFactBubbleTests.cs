using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class FunFactBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayFactAndLink_WhenJsonIsValidAndHasUrl()
        {
            var payload = new
            {
                Text = "Honey never spoils. You can theoretically eat 3,000-year-old honey.",
                SourceUrl = "https://example.com/honey"
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<FunFactBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("div").TextContent.Should().Contain("Honey never spoils.");

            var link = cut.Find("a");
            link.GetAttribute("href").Should().Be("https://example.com/honey");
            link.TextContent.Trim().Should().Be("Click here");
            cut.FindAll(".text-content").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldNotDisplayLink_WhenSourceUrlIsEmptyOrWhitespace()
        {
            var payload = new
            {
                Text = "Bananas are berries, but strawberries aren't.",
                SourceUrl = "   "
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<FunFactBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("div").TextContent.Should().Contain("Bananas are berries");
            cut.FindAll("a").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldFallbackToRawContentAsMarkup_WhenJsonIsMalformed()
        {
            string rawHtmlContent = "Did you know that <i>sea otters</i> hold hands while sleeping?";

            var cut = Render<FunFactBubble>(parameters => parameters
                .Add(p => p.RawContent, rawHtmlContent)
            );

            var fallbackContainer = cut.FindAll(".text-content")[0];
            fallbackContainer.TextContent.Should().Be("Did you know that sea otters hold hands while sleeping?");
            fallbackContainer.QuerySelector("i")!.TextContent.Should().Be("sea otters");
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"text\":\"Gamer fact text.\",\"sourceurl\":\"\"}";

            var cut = Render<FunFactBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
            );

            cut.Find("div").TextContent.Should().Contain("Gamer fact text.");
        }
    }
}