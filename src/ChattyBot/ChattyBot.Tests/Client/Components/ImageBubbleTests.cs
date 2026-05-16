using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Client.Tests.Components
{
    public class ImageBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayImageWithCorrectAttributes_WhenJsonIsValid()
        {
            var payload = new
            {
                Url = "images/meme.png",
                AltText = "Funny programming meme"
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<ImageBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            var img = cut.Find("img");
            img.GetAttribute("src").Should().Be("images/meme.png");
            img.GetAttribute("alt").Should().Be("Funny programming meme");
            img.ClassList.Should().Contain("chat-meme-img");
            cut.FindAll(".text-content").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldDisplayErrorMessage_WhenJsonIsMalformed()
        {
            string invalidJson = "{ corrupt json }";

            var cut = Render<ImageBubble>(parameters => parameters
                .Add(p => p.RawContent, invalidJson)
            );

            var errorDiv = cut.FindAll(".text-content")[0];
            errorDiv.TextContent.Trim().Should().Be("Image load error.");
            cut.FindAll("img").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"url\":\"images/test.jpg\",\"alttext\":\"test alt\"}";

            var cut = Render<ImageBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
            );

            var img = cut.Find("img");
            img.GetAttribute("src").Should().Be("images/test.jpg");
            img.GetAttribute("alt").Should().Be("test alt");
        }
    }
}