using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Server.Commands
{
    public class JokeCommandTests
    {
        private readonly IJokeRepository _jokeRepo;
        private readonly JokeCommand _sut;

        public JokeCommandTests()
        {
            _jokeRepo = Substitute.For<IJokeRepository>();
            _sut = new JokeCommand(_jokeRepo);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/joke");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnJoke_WhenRepositoryHasData()
        {
            var expectedJoke = "Why did the programmer quit his job? Because he didn't get arrays.";
            _jokeRepo.GetRandomAsync().Returns(new Joke { Content = expectedJoke });

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Be(expectedJoke);

            await _jokeRepo.Received(1).GetRandomAsync();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenNoJokesFound()
        {
            _jokeRepo.GetRandomAsync().Returns((Joke)null!);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I'm fresh out of jokes");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndStillReturnJoke()
        {
            _jokeRepo.GetRandomAsync().Returns(new Joke { Content = "Funny joke" });

            var result = await _sut.ExecuteAsync("some random text");

            result.Should().NotBeNull();
            result.Content.Should().Be("Funny joke");
        }
    }
}