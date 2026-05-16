using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Client.Tests.Components
{
    public class CoinBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayHeadsImmediately_WhenIsNewIsFalseAndResultIsHeads()
        {
            var payload = new { Result = "Heads" };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<CoinFlipBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            cut.Find("#coin").ClassList.Should().Contain("static-heads");
            cut.Find(".coin-label").TextContent.Trim().Should().Be("It's Heads!");
        }

        [Fact]
        public void Render_ShouldDisplayTailsImmediately_WhenIsNewIsFalseAndResultIsTails()
        {
            var payload = new { Result = "Tails" };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<CoinFlipBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            cut.Find("#coin").ClassList.Should().Contain("static-tails");
            cut.Find(".coin-label").TextContent.Trim().Should().Be("It's Tails!");
        }

        [Fact]
        public void Render_ShouldAnimateThenShowResult_WhenIsNewIsTrue()
        {
            var payload = new { Result = "Heads" };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<CoinFlipBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, true)
            );

            cut.Find("#coin").ClassList.Should().Contain("heads-anim");
            cut.FindAll(".coin-label").Should().BeEmpty();

            cut.WaitForAssertion(() =>
            {
                cut.Find("#coin").ClassList.Should().Contain("static-heads");
                cut.Find(".coin-label").TextContent.Trim().Should().Be("It's Heads!");
            }, TimeSpan.FromSeconds(4));
        }

        [Fact]
        public void Render_ShouldNotCrash_WhenJsonIsMalformed()
        {
            string malformedJson = "{ invalid json }";

            var cut = Render<CoinFlipBubble>(parameters => parameters
                .Add(p => p.RawContent, malformedJson)
                .Add(p => p.IsNew, false)
            );

            cut.Find("#coin").ClassList.Should().Contain("static-tails");
            cut.FindAll(".coin-label").Should().BeEmpty();
        }
    }
}