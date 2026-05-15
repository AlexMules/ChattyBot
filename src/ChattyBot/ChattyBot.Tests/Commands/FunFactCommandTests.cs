using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Commands
{
    public class FunFactCommandTests
    {
        private readonly IFunFactRepository _repo;
        private readonly FunFactCommand _sut;

        public FunFactCommandTests()
        {
            _repo = Substitute.For<IFunFactRepository>();
            _sut = new FunFactCommand(_repo);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/funfact");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFact_WhenRepositoryHasData()
        {
            var fakeFact = new FunFact
            {
                Content = "Honey never spoils.",
                SourceUrl = "https://example.com"
            };

            _repo.GetRandomAsync().Returns(fakeFact);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.FunFact);

            var payload = JsonDocument.Parse(result.Content).RootElement;
            payload.GetProperty("Text").GetString().Should().Be("Honey never spoils.");
            payload.GetProperty("SourceUrl").GetString().Should().Be("https://example.com");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenNoFactsFound()
        {
            _repo.GetRandomAsync().Returns((FunFact)null!);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I couldn't find any interesting facts");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndStillReturnFact()
        {
            _repo.GetRandomAsync().Returns(new FunFact { Content = "Test", SourceUrl = "URL" });

            var result = await _sut.ExecuteAsync("some random input");

            await _repo.Received(1).GetRandomAsync();
            result.Type.Should().Be(MessageType.FunFact);
        }
    }
}