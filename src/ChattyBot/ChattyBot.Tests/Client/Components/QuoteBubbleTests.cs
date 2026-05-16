using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class QuoteBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayQuoteAndAuthorAndLink_WhenJsonIsValidAndHasUrl()
        {
            var payload = new
            {
                Text = "The only limit to our realization of tomorrow is our doubts of today.",
                Author = "Franklin D. Roosevelt",
                SourceUrl = "https://example.com/quote"
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<QuoteBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("div").TextContent.Should().Contain("The only limit to our realization of tomorrow is our doubts of today.");
            cut.Find("strong").TextContent.Should().Be("Franklin D. Roosevelt");

            var link = cut.Find("a");
            link.GetAttribute("href").Should().Be("https://example.com/quote");
            link.TextContent.Trim().Should().Be("Click here");
        }

        [Fact]
        public void Render_ShouldNotDisplayLink_WhenSourceUrlIsEmptyOrWhitespace()
        {
            var payload = new
            {
                Text = "Simplicity is the ultimate sophistication.",
                Author = "Leonardo da Vinci",
                SourceUrl = "   "
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<QuoteBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("strong").TextContent.Should().Be("Leonardo da Vinci");
            cut.FindAll("a").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldFallbackToRawContentAsMarkup_WhenJsonIsMalformed()
        {
            string rawHtmlContent = "This is a <b>plain text</b> fallback message.";

            var cut = Render<QuoteBubble>(parameters => parameters
                .Add(p => p.RawContent, rawHtmlContent)
            );

            var fallbackContainer = cut.FindAll(".text-content")[0];
            fallbackContainer.TextContent.Should().Be("This is a plain text fallback message.");

            fallbackContainer.QuerySelector("b")!.TextContent.Should().Be("plain text");

            cut.FindAll("strong").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"text\":\"Stay hungry, stay foolish.\",\"author\":\"Steve Jobs\",\"sourceurl\":\"\"}";

            var cut = Render<QuoteBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
            );

            cut.Find("strong").TextContent.Should().Be("Steve Jobs");
        }
    }
}