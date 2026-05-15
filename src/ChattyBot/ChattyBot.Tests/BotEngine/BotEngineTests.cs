using ChattyBot.Server.Application.BotEngine;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.BotEngine
{
    public class BotEngineTests
    {
        private readonly List<IBotCommand> _mockCommands;
        private readonly BotCommandContext _context;
        private readonly Server.Application.BotEngine.BotEngine _sut;

        public BotEngineTests()
        {
            var jokeCommand = Substitute.For<IBotCommand>();
            jokeCommand.CommandTrigger.Returns("/joke");

            _mockCommands = new List<IBotCommand> { jokeCommand };
            _context = new BotCommandContext();

            _sut = new Server.Application.BotEngine.BotEngine(_mockCommands, _context);
        }

        [Fact]
        public void GetWelcomeMessage_ShouldReturnPersonalizedGreeting()
        {
            var result = Server.Application.BotEngine.BotEngine.GetWelcomeMessage("Alex");

            result.Content.Should().Contain("Hi there, Alex!");
            result.Type.Should().Be(MessageType.Text);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ResolveAndExecuteAsync_InvalidInput_ShouldReturnHelpPrompt(string? input)
        {
            var result = await _sut.ResolveAndExecuteAsync(input!, "User");

            result.Content.Should().Contain("Please enter a command");
            result.Type.Should().Be(MessageType.Text);
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_MissingSlash_ShouldReturnFormatError()
        {
            var result = await _sut.ResolveAndExecuteAsync("joke", "User");

            result.Content.Should().Contain("Invalid format");
            result.Content.Should().Contain("must start with a '/'");
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_CommandNotFound_ShouldReturnUnknownCommandMessage()
        {
            var result = await _sut.ResolveAndExecuteAsync("/nonexistent", "User");

            result.Content.Should().Contain("I don't recognize the command '/nonexistent'");
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_ShouldBeCaseInsensitive()
        {
            _mockCommands[0].ExecuteAsync(Arg.Any<string>()).Returns(new BotResponse("ok", MessageType.Text));

            var result = await _sut.ResolveAndExecuteAsync("/JOKE", "User");

            result.Content.Should().Be("ok");
            await _mockCommands[0].Received(1).ExecuteAsync(null);
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_WithParameters_ShouldPassThemToCommand()
        {
            await _sut.ResolveAndExecuteAsync("/joke knock knock", "User");

            await _mockCommands[0].Received(1).ExecuteAsync("knock knock");
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_ShouldUpdateContextUsername()
        {
            await _sut.ResolveAndExecuteAsync("/joke", "Alex");

            _context.Username.Should().Be("Alex");
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_JustASlash_ShouldReturnUnknownCommand()
        {
            var result = await _sut.ResolveAndExecuteAsync("/", "User");

            result.Content.Should().Contain("I don't recognize the command '/'");
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_InputWithLeadingTrailingSpaces_ShouldStillWork()
        {
            await _sut.ResolveAndExecuteAsync("   /joke   ", "User");

            await _mockCommands[0].Received(1).ExecuteAsync(null);
        }

        [Fact]
        public async Task ResolveAndExecuteAsync_ShouldRouteToCorrectCommand_WhenMultipleCommandsExist()
        {
            var jokeCmd = Substitute.For<IBotCommand>();
            jokeCmd.CommandTrigger.Returns("/joke");

            var calcCmd = Substitute.For<IBotCommand>();
            calcCmd.CommandTrigger.Returns("/calc");

            var diceCmd = Substitute.For<IBotCommand>();
            diceCmd.CommandTrigger.Returns("/dice");

            var localCommands = new List<IBotCommand> { jokeCmd, calcCmd, diceCmd };
            var localSut = new ChattyBot.Server.Application.BotEngine.BotEngine(localCommands, _context);

            await localSut.ResolveAndExecuteAsync("/calc 2+2", "User");

            await calcCmd.Received(1).ExecuteAsync("2+2");

            await jokeCmd.DidNotReceive().ExecuteAsync(Arg.Any<string>());
            await diceCmd.DidNotReceive().ExecuteAsync(Arg.Any<string>());
        }
    }
}