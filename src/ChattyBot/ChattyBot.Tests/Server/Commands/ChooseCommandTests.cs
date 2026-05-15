using ChattyBot.Server.Application.BotEngine.Commands;
using FluentAssertions;

namespace ChattyBot.Tests.Server.Commands
{
    public class ChooseCommandTests
    {
        private readonly ChooseCommand _sut;

        public ChooseCommandTests()
        {
            _sut = new ChooseCommand();
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/choose");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ExecuteAsync_ShouldReturnUsageMessage_WhenParametersAreMissing(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("Please provide options separated by commas");
        }

        [Theory]
        [InlineData("Pizza")] 
        [InlineData("Pizza, ")] 
        [InlineData(", , ,")] 
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenFewerThanTwoValidOptions(string input)
        {
            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().Contain("I need at least two options");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldPickOneOfProvidedOptions()
        {
            var input = "Pizza, Burger, Sushi";
            var expectedOptions = new[] { "Pizza", "Burger", "Sushi" };

            var result = await _sut.ExecuteAsync(input);

            result.Content.Should().StartWith("I choose: ");

            var chosenOption = result.Content.Replace("I choose: ", "");

            expectedOptions.Should().Contain(chosenOption);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleExtraSpacesAndEmptyEntries()
        {
            var input = "  Laptop  , ,   Smartphone  ";

            var result = await _sut.ExecuteAsync(input);

            var chosenOption = result.Content.Replace("I choose: ", "");

            new[] { "Laptop", "Smartphone" }.Should().Contain(chosenOption);
            chosenOption.Should().NotStartWith(" ").And.NotEndWith(" ");
        }
    }
}