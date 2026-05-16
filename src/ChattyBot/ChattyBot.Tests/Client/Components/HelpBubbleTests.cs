using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Client.Tests.Components
{
    public class HelpBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayCommandsAndSplitDescriptions_WhenJsonIsValid()
        {
            var payload = new[]
            {
                new { Trigger = "/help", Description = "Show help menu|Usage: /help [command]" }
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<HelpBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.FindAll(".help-container").Should().NotBeEmpty();

            var containerText = cut.Find(".help-container").TextContent;
            containerText.Should().Contain("/help");
            containerText.Should().Contain("Usage: /help [command]");

            var spans = cut.FindAll("span");
            spans.Count.Should().Be(1);
            spans[0].TextContent.Trim().Should().Be("Show help menu");
        }

        [Fact]
        public void Render_ShouldNotDisplayItalicDetails_WhenDescriptionHasNoPipe()
        {
            var payload = new[]
            {
                new { Trigger = "/joke", Description = "Tell a random joke" }
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<HelpBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            var spans = cut.FindAll("span");
            spans.Count.Should().Be(1);
            spans[0].TextContent.Trim().Should().Be("Tell a random joke");

            cut.Find(".help-container").TextContent.Should().NotContain("|");
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "[{\"trigger\":\"/clear\",\"description\":\"Clear chat history\"}]";

            var cut = Render<HelpBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
            );

            var text = cut.Find(".help-container").TextContent;
            text.Should().Contain("/clear");
            text.Should().Contain("Clear chat history");
        }

        [Fact]
        public void Render_ShouldRenderEmpty_WhenJsonIsMalformed()
        {
            string invalidJson = "{ invalid }";

            var cut = Render<HelpBubble>(parameters => parameters
                .Add(p => p.RawContent, invalidJson)
            );

            cut.Nodes.Should().BeEmpty();
        }
    }
}