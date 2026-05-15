using ChattyBot.Server.Commands;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Commands
{
    public class MusicCommandTests
    {
        private readonly ISongRepository _repo;
        private readonly MusicCommand _sut;

        public MusicCommandTests()
        {
            _repo = Substitute.For<ISongRepository>();
            _sut = new MusicCommand(_repo);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/music");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_NoParams_ShouldReturnRandomSong()
        {
            var song = new Song { Title = "Song1", Artist = "Artist1", SongPath = "path" };
            _repo.GetRandomAsync().Returns(song);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Music);
            result.Content.Should().Contain("Song1");
            await _repo.Received(1).GetRandomAsync();
        }

        [Theory]
        [InlineData("-rock", MusicCategory.Rock)]
        [InlineData("-pop", MusicCategory.Pop)]
        [InlineData("-jazz", MusicCategory.Jazz)]
        [InlineData("-rap", MusicCategory.Rap)]
        public async Task ExecuteAsync_ValidCategory_ShouldCallCorrectRepoMethod(string param, MusicCategory expectedCat)
        {
            var song = new Song { Title = "GenreSong", Artist = "Artist", SongPath = "path" };
            _repo.GetRandomByCategoryAsync(expectedCat).Returns(song);

            var result = await _sut.ExecuteAsync(param);

            result.Type.Should().Be(MessageType.Music);
            await _repo.Received(1).GetRandomByCategoryAsync(expectedCat);
        }

        [Fact]
        public async Task ExecuteAsync_InvalidCategory_ShouldReturnErrorMessage()
        {
            var result = await _sut.ExecuteAsync("-unknown");

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I don't recognize that genre");
            await _repo.DidNotReceive().GetRandomAsync();
            await _repo.DidNotReceive().GetRandomByCategoryAsync(Arg.Any<MusicCategory>());
        }

        [Fact]
        public async Task ExecuteAsync_SongNotFound_ShouldReturnErrorMessage()
        {
            _repo.GetRandomAsync().Returns((Song)null!);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I couldn't find any songs");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnCorrectJsonStructure()
        {
            var song = new Song
            {
                Title = "TestTitle",
                Artist = "TestArtist",
                Description = "TestDesc",
                SongPath = "TestPath"
            };
            _repo.GetRandomAsync().Returns(song);

            var result = await _sut.ExecuteAsync();

            var payload = JsonDocument.Parse(result.Content).RootElement;
            payload.GetProperty("Title").GetString().Should().Be("TestTitle");
            payload.GetProperty("Artist").GetString().Should().Be("TestArtist");
            payload.GetProperty("Description").GetString().Should().Be("TestDesc");
            payload.GetProperty("SongPath").GetString().Should().Be("TestPath");
        }
    }
}