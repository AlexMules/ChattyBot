using ChattyBot.Server.Application.BotEngine;
using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class HelpCommandTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HelpCommand _sut;

        public HelpCommandTests()
        {
            _serviceProvider = Substitute.For<IServiceProvider>();
            _sut = new HelpCommand(_serviceProvider);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/help");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnListOfCommands_OrderedByTrigger()
        {
            var cmd1 = Substitute.For<IBotCommand>();
            cmd1.CommandTrigger.Returns("/zebra");
            cmd1.Description.Returns("Zebra desc");

            var cmd2 = Substitute.For<IBotCommand>();
            cmd2.CommandTrigger.Returns("/alpha");
            cmd2.Description.Returns("Alpha desc");

            var commands = new List<IBotCommand> { cmd1, cmd2 };

            _serviceProvider.GetService(typeof(IEnumerable<IBotCommand>)).Returns(commands);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Help);

            var payload = JsonSerializer.Deserialize<List<CommandHelpInfo>>(result.Content);

            payload.Should().NotBeNull();
            payload.Should().HaveCount(2);

            payload![0].Trigger.Should().Be("/alpha");
            payload![1].Trigger.Should().Be("/zebra");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyJson_WhenNoCommandsAreRegistered()
        {
            _serviceProvider.GetService(typeof(IEnumerable<IBotCommand>)).Returns(Enumerable.Empty<IBotCommand>());

            var result = await _sut.ExecuteAsync();

            result.Content.Should().Be("[]");
            result.Type.Should().Be(MessageType.Help);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndReturnAllCommands()
        {
            _serviceProvider.GetService(typeof(IEnumerable<IBotCommand>))
                            .Returns(new List<IBotCommand> { Substitute.For<IBotCommand>() });

            var result = await _sut.ExecuteAsync("some extra text");

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.Help);
        }

        private class CommandHelpInfo
        {
            public string Trigger { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}