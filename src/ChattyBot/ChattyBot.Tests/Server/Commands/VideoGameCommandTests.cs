using ChattyBot.Server.Commands;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class VideoGameCommandTests
    {
        private readonly IVideoGameRepository _repository;
        private readonly VideoGameCommand _sut;

        public VideoGameCommandTests()
        {
            _repository = Substitute.For<IVideoGameRepository>();
            _sut = new VideoGameCommand(_repository);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/videogame");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_NoParams_ShouldCallGetRandomAsync()
        {
            var game = new VideoGame { Title = "Elden Ring", Description = "Masterpiece", ImagePath = "path/to/img" };
            _repository.GetRandomAsync().Returns(game);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Videogame);
            result.Content.Should().Contain("Elden Ring");
            await _repository.Received(1).GetRandomAsync();
        }

        [Theory]
        [InlineData("-fps", GameCategory.FPS)]
        [InlineData("-rpg", GameCategory.RPG)]
        [InlineData("-action-adventure", GameCategory.ActionAdventure)]
        [InlineData("-simulation", GameCategory.Simulation)]
        public async Task ExecuteAsync_ValidCategory_ShouldCallGetRandomByCategoryAsync(string param, GameCategory expectedCategory)
        {
            var game = new VideoGame { Title = "Category Game", Description = "Desc", ImagePath = "img" };
            _repository.GetRandomByCategoryAsync(expectedCategory).Returns(game);

            var result = await _sut.ExecuteAsync(param);

            result.Type.Should().Be(MessageType.Videogame);
            await _repository.Received(1).GetRandomByCategoryAsync(expectedCategory);
        }

        [Fact]
        public async Task ExecuteAsync_InvalidCategory_ShouldReturnErrorMessage()
        {
            var result = await _sut.ExecuteAsync("-unknown-category");

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I don't recognize that category");
            await _repository.DidNotReceive().GetRandomAsync();
            await _repository.DidNotReceive().GetRandomByCategoryAsync(Arg.Any<GameCategory>());
        }

        [Fact]
        public async Task ExecuteAsync_GameNotFound_ShouldReturnErrorMessage()
        {
            _repository.GetRandomAsync().Returns((VideoGame?)null);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("No games found");
        }

        [Fact]
        public async Task ExecuteAsync_Success_ShouldReturnCorrectJsonPayload()
        {
            var game = new VideoGame
            {
                Title = "The Witcher 3",
                Description = "Geralt's adventures",
                ImagePath = "witcher3.jpg"
            };
            _repository.GetRandomAsync().Returns(game);

            var result = await _sut.ExecuteAsync();

            var payload = JsonDocument.Parse(result.Content).RootElement;
            payload.GetProperty("Title").GetString().Should().Be("The Witcher 3");
            payload.GetProperty("Description").GetString().Should().Be("Geralt's adventures");
            payload.GetProperty("ImagePath").GetString().Should().Be("witcher3.jpg");
        }
    }
}