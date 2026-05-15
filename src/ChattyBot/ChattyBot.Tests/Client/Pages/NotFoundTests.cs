using Bunit;
using ChattyBot.Client.Pages; 
using FluentAssertions;

namespace ChattyBot.Client.Tests.Pages
{
    public class NotFoundTests : BunitContext
    {
        [Fact]
        public void NotFoundPage_ShouldRenderCorrectly_AndHaveHomeLink()
        {
            var cut = Render<NotFound>();

            cut.Find("h3").TextContent.Should().Be("Oops! You're lost.");

            var backButton = cut.Find("a.btn-blurple");
            backButton.GetAttribute("href").Should().Be("home");
        }
    }
}