using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class VideoGameBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayFullGameDetails_WhenJsonIsValid()
        {
            var payload = new
            {
                Title = "The Witcher 3",
                Description = "A masterpiece RPG.",
                ImagePath = "images/witcher3.png"
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<VideoGameBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("h5").TextContent.Trim().Should().Be("The Witcher 3");
            cut.Find("p").TextContent.Trim().Should().Be("A masterpiece RPG.");

            var img = cut.Find("img");
            img.GetAttribute("src").Should().Be("images/witcher3.png");
            img.GetAttribute("alt").Should().Be("Game Poster");
        }

        [Fact]
        public void Render_ShouldNotDisplayImage_WhenImagePathIsEmpty()
        {
            var payload = new
            {
                Title = "No Image Game",
                Description = "A text-only description.",
                ImagePath = ""
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<VideoGameBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("h5").TextContent.Trim().Should().Be("No Image Game");
            cut.FindAll("img").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldDisplayErrorMessage_WhenJsonIsMalformed()
        {
            string invalidJson = "{ this is not json }";

            var cut = Render<VideoGameBubble>(parameters => parameters
                .Add(p => p.RawContent, invalidJson)
            );

            var errorDiv = cut.Find("div");
            errorDiv.TextContent.Should().Contain("[Error loading game data]");
            errorDiv.GetAttribute("style").Should().Contain("#ff4d4d");

            cut.FindAll("h5").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"title\":\"Cyberpunk 2077\",\"description\":\"Night City awaits.\",\"imagepath\":\"cp.png\"}";

            var cut = Render<VideoGameBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
            );

            cut.Find("h5").TextContent.Should().Be("Cyberpunk 2077");
            cut.Find("img").GetAttribute("src").Should().Be("cp.png");
        }
    }
}