using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Commands
{
    public class QuoteCommandTests
    {
        private readonly IQuoteRepository _quoteRepo;
        private readonly QuoteCommand _sut;

        public QuoteCommandTests()
        {
            _quoteRepo = Substitute.For<IQuoteRepository>();
            _sut = new QuoteCommand(_quoteRepo);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/quote");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnQuotePayload_WhenRepositoryHasData()
        {
            var expectedQuote = new Quote
            {
                Text = "To be or not to be.",
                Author = "Shakespeare",
                SourceUrl = "https://example.com"
            };
            _quoteRepo.GetRandomAsync().Returns(expectedQuote);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Quote);

            var payload = JsonDocument.Parse(result.Content).RootElement;
            payload.GetProperty("Text").GetString().Should().Be(expectedQuote.Text);
            payload.GetProperty("Author").GetString().Should().Be(expectedQuote.Author);
            payload.GetProperty("SourceUrl").GetString().Should().Be(expectedQuote.SourceUrl);

            await _quoteRepo.Received(1).GetRandomAsync();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnErrorMessage_WhenNoQuotesFound()
        {
            _quoteRepo.GetRandomAsync().Returns((Quote)null!);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("book of wisdom is currently empty");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIgnoreParameters_AndWorkNormally()
        {
            _quoteRepo.GetRandomAsync().Returns(new Quote { Text = "Test", Author = "A", SourceUrl = "S" });

            var result = await _sut.ExecuteAsync("some random input");

            result.Should().NotBeNull();
            result.Type.Should().Be(MessageType.Quote);
            result.Content.Should().Contain("Test");
        }
    }
}