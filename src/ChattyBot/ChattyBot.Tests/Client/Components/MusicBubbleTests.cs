using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class MusicBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayFullSongDetailsAndLink_WhenJsonIsValidAndHasPath()
        {
            var payload = new
            {
                Title = "Blinding Lights",
                Artist = "The Weeknd",
                Description = "An iconic synth-wave track.",
                SongPath = "https://youtube.com/watch?v=4NRXx6U8ABQ"
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<MusicBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("h5").TextContent.Trim().Should().Be("Blinding Lights");
            cut.Find("strong").TextContent.Trim().Should().Be("The Weeknd");
            cut.Find("p").TextContent.Trim().Should().Be("An iconic synth-wave track.");

            var link = cut.Find("a");
            link.GetAttribute("href").Should().Be("https://youtube.com/watch?v=4NRXx6U8ABQ");
            link.TextContent.Should().Contain("Listen on YouTube");
        }

        [Fact]
        public void Render_ShouldNotDisplayLink_WhenSongPathIsEmpty()
        {
            var payload = new
            {
                Title = "Instrumental Track",
                Artist = "Unknown Artist",
                Description = "Just audio.",
                SongPath = ""
            };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<MusicBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            cut.Find("h5").TextContent.Trim().Should().Be("Instrumental Track");
            cut.FindAll("a").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldRenderEmptyMarkup_WhenJsonIsMalformed()
        {
            string invalidJson = "{ invalid json }";

            var cut = Render<MusicBubble>(parameters => parameters
                .Add(p => p.RawContent, invalidJson)
            );

            cut.Nodes.Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"title\":\"Starboy\",\"artist\":\"The Weeknd\",\"description\":\"Hit song.\",\"songpath\":\"\"}";

            var cut = Render<MusicBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
            );

            cut.Find("h5").TextContent.Trim().Should().Be("Starboy");
            cut.Find("strong").TextContent.Trim().Should().Be("The Weeknd");
        }
    }
}