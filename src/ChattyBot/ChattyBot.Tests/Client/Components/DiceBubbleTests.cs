using Bunit;
using ChattyBot.Client.Components;
using FluentAssertions;
using System.Text.Json;

namespace ChattyBot.Client.Tests.Components
{
    public class DiceBubbleTests : BunitContext
    {
        [Fact]
        public void Render_ShouldDisplayDiceValuesImmediately_WhenIsNewIsFalse()
        {
            var payload = new { Die1 = 3, Die2 = 4 };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<DiceBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, false)
            );

            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("3");
            dice[1].TextContent.Trim().Should().Be("4");
            dice[0].ClassList.Should().NotContain("die-rolling");
            dice[1].ClassList.Should().NotContain("die-rolling");

            cut.Find(".dice-result-text").TextContent.Trim().Should().Be("You rolled a 7!");
            cut.FindAll(".dice-error").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldAnimateAndThenShowFinalResults_WhenIsNewIsTrue()
        {
            var payload = new { Die1 = 2, Die2 = 6 };
            string json = JsonSerializer.Serialize(payload);

            var cut = Render<DiceBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.IsNew, true)
            );

            var initialDice = cut.FindAll(".die");
            initialDice[0].ClassList.Should().Contain("die-rolling");
            initialDice[1].ClassList.Should().Contain("die-rolling");
            cut.FindAll(".dice-result-text").Should().BeEmpty();

            cut.WaitForAssertion(() =>
            {
                var finalDice = cut.FindAll(".die");
                finalDice[0].ClassList.Should().NotContain("die-rolling");
                finalDice[1].ClassList.Should().NotContain("die-rolling");
                finalDice[0].TextContent.Trim().Should().Be("2");
                finalDice[1].TextContent.Trim().Should().Be("6");

                cut.Find(".dice-result-text").TextContent.Trim().Should().Be("You rolled a 8!");
            }, TimeSpan.FromSeconds(3));
        }

        [Fact]
        public void Render_ShouldDisplayErrorMessage_WhenJsonIsMalformed()
        {
            string invalidJson = "{ invalid json }";

            var cut = Render<DiceBubble>(parameters => parameters
                .Add(p => p.RawContent, invalidJson)
                .Add(p => p.IsNew, false)
            );

            cut.Find(".dice-error").TextContent.Should().Contain("Dice data error.");
            cut.FindAll(".die").Should().BeEmpty();
            cut.FindAll(".dice-result-text").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldBeCaseInsensitive_WhenParsingJson()
        {
            string lowercaseJson = "{\"die1\":5,\"die2\":5}";

            var cut = Render<DiceBubble>(parameters => parameters
                .Add(p => p.RawContent, lowercaseJson)
                .Add(p => p.IsNew, false)
            );

            var dice = cut.FindAll(".die");
            dice[0].TextContent.Trim().Should().Be("5");
            dice[1].TextContent.Trim().Should().Be("5");
            cut.Find(".dice-result-text").TextContent.Trim().Should().Be("You rolled a 10!");
        }
    }
}