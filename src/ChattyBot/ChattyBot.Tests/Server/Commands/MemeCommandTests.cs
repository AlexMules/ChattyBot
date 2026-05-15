using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class MemeCommandTests
    {
        private readonly IMemeRepository _memeRepo;
        private readonly MemeCommand _sut;

        public MemeCommandTests()
        {
            _memeRepo = Substitute.For<IMemeRepository>();
            _sut = new MemeCommand(_memeRepo);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/meme");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMemePayload_WhenRepositoryHasData()
        {
            var expectedUrl = "https://chattybot.com/memes/coding.jpg";
            var fakeMeme = new Meme { ImagePath = expectedUrl };
            _memeRepo.GetRandomAsync().Returns(fakeMeme);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Image);

            var payload = JsonDocument.Parse(result.Content).RootElement;
            payload.GetProperty("Url").GetString().Should().Be(expectedUrl);

            await _memeRepo.Received(1).GetRandomAsync();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenNoMemesFound()
        {
            _memeRepo.GetRandomAsync().Returns((Meme)null!);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I ran out of memes");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndWorkNormally()
        {
            _memeRepo.GetRandomAsync().Returns(new Meme { ImagePath = "url" });

            var result = await _sut.ExecuteAsync("unnecessary input");

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.Image);
            result.Content.Should().Contain("url");
        }
    }
}