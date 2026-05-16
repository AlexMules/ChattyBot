using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class RpsBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayChoicesImmediately_WhenIsNewIsFalse()
        {
            var payload = new { UserChoice = "scissors", BotChoice = "paper" };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<RpsBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            cut.Find("input").GetAttribute("id").Should().Be("scissors-paper");
            cut.Find("#user-hand").ClassList.Should().NotContain("is-shaking");
            cut.Find("#computer-hand").ClassList.Should().NotContain("is-shaking");
            cut.Find("#message p").TextContent.Trim().Should().Contain("You: scissors vs Bot: paper");
        }

        [Fact]
        public void Render_ShouldFallbackToEmpty_WhenPropertiesAreMissingInJson()
        {
            string emptyJson = "{}";

            var cut = Render<RpsBubble>(parameters => parameters
                .Add(p => p.RawContent, emptyJson)
                .Add(p => p.IsNew, false)
            );

            cut.Find("input").GetAttribute("id").Should().Be("-");
            cut.Find("#message p").TextContent.Trim().Should().Contain("You:  vs Bot:");
        }

        [Fact]
        public void Render_ShouldTriggerAnimationThenShowResult_WhenIsNewIsTrue()
        {
            var payload = new { UserChoice = "paper", BotChoice = "rock" };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<RpsBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, true)
            );

            cut.Find("input").GetAttribute("id").Should().Be("rock-rock");
            cut.Find("#user-hand").ClassList.Should().Contain("is-shaking");
            cut.Find("#computer-hand").ClassList.Should().Contain("is-shaking");
            cut.FindAll("#message").Should().BeEmpty();

            cut.WaitForAssertion(() =>
            {
                cut.Find("input").GetAttribute("id").Should().Be("paper-rock");
                cut.Find("#user-hand").ClassList.Should().NotContain("is-shaking");
                cut.Find("#computer-hand").ClassList.Should().NotContain("is-shaking");
                cut.Find("#message p").TextContent.Trim().Should().Contain("You: paper vs Bot: rock");
            }, TimeSpan.FromSeconds(3));
        }

        [Fact]
        public void Render_ShouldNotCrash_WhenJsonIsMalformed()
        {
            string malformedJson = "{ invalid json }";

            var cut = Render<RpsBubble>(parameters => parameters
                .Add(p => p.RawContent, malformedJson)
                .Add(p => p.IsNew, false)
            );

            cut.Find("input").GetAttribute("id").Should().Be("-");
            cut.Find("#user-hand").ClassList.Should().NotContain("is-shaking");
        }
    }
}