using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class DiceDuelBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayUserVictoryImmediately_WhenIsNewIsFalseAndUserRollIsHigher()
        {
            var payload = new { UserRoll = 6, BotRoll = 3 };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<DiceDuelBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("6");
            dice[1].TextContent.Trim().Should().Be("3");
            dice[0].ClassList.Should().NotContain("die-rolling");
            dice[1].ClassList.Should().NotContain("die-rolling");

            var result = cut.Find(".duel-result");
            result.ClassList.Should().Contain("win");
            result.TextContent.Trim().Should().Be("You win!");
        }

        [Fact]
        public void Render_ShouldDisplayBotVictoryImmediately_WhenIsNewIsFalseAndBotRollIsHigher()
        {
            var payload = new { UserRoll = 2, BotRoll = 5 };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<DiceDuelBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("2");
            dice[1].TextContent.Trim().Should().Be("5");

            var result = cut.Find(".duel-result");
            result.ClassList.Should().Contain("loss");
            result.TextContent.Trim().Should().Be("ChattyBot wins!");
        }

        [Fact]
        public void Render_ShouldDisplayDrawImmediately_WhenIsNewIsFalseAndRollsAreEqual()
        {
            var payload = new { UserRoll = 4, BotRoll = 4 };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<DiceDuelBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("4");
            dice[1].TextContent.Trim().Should().Be("4");

            var result = cut.Find(".duel-result");
            result.ClassList.Should().Contain("draw");
            result.TextContent.Trim().Should().Be("It's a draw!");
        }

        [Fact]
        public void Render_ShouldAnimateAndThenShowFinalResults_WhenIsNewIsTrue()
        {
            var payload = new { UserRoll = 5, BotRoll = 1 };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<DiceDuelBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, true)
            );

            var initialDice = cut.FindAll(".die");
            initialDice[0].ClassList.Should().Contain("die-rolling");
            initialDice[1].ClassList.Should().Contain("die-rolling");
            cut.FindAll(".duel-result").Should().BeEmpty();

            cut.WaitForAssertion(() =>
            {
                var finalDice = cut.FindAll(".die");
                finalDice[0].ClassList.Should().NotContain("die-rolling");
                finalDice[1].ClassList.Should().NotContain("die-rolling");
                finalDice[0].TextContent.Trim().Should().Be("5");
                finalDice[1].TextContent.Trim().Should().Be("1");

                var result = cut.Find(".duel-result");
                result.ClassList.Should().Contain("win");
                result.TextContent.Trim().Should().Be("You win!");
            }, TimeSpan.FromSeconds(3));
        }

        [Fact]
        public void Render_ShouldNotCrashAndHideResult_WhenJsonIsMalformed()
        {
            string malformedJson = "{ invalid }";

            var cut = Render<DiceDuelBubble>(parameters => parameters
                .Add(p => p.RawContent, malformedJson)
                .Add(p => p.IsNew, false)
            );

            cut.FindAll(".duel-result").Should().BeEmpty();
            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("0");
            dice[1].TextContent.Trim().Should().Be("0");
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"userroll\":3,\"botroll\":6}";

            var cut = Render<DiceDuelBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
                .Add(p => p.IsNew, false)
            );

            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("3");
            dice[1].TextContent.Trim().Should().Be("6");
            cut.Find(".duel-result").TextContent.Trim().Should().Be("ChattyBot wins!");
        }
    }
}