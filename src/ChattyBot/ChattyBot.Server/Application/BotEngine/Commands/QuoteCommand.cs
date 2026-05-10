using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class QuoteCommand : IBotCommand
    {
        private readonly IQuoteRepository _quoteRepo;

        public string CommandTrigger => "/quote";
        public string Description => "Get an inspirational quote and learn about its author!";

        public QuoteCommand(IQuoteRepository quoteRepo)
        {
            _quoteRepo = quoteRepo;
        }

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var quote = await _quoteRepo.GetRandomAsync();

            if (quote == null)
            {
                return new BotResponse("My book of wisdom is currently empty. Try again later!", MessageType.Text);
            }

            var payload = new
            {
                Text = quote.Text,
                Author = quote.Author,
                SourceUrl = quote.SourceUrl
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.Quote);
        }
    }
}