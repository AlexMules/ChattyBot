using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using System.Text;

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

        public async Task<string> ExecuteAsync(string? parameters = null)
        {
            var quote = await _quoteRepo.GetRandomAsync();

            if (quote == null)
            {
                return "My book of wisdom is currently empty. Try again later!";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"\"{quote.Text}\"");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"— <b>{quote.Author}</b>");

            if (!string.IsNullOrWhiteSpace(quote.SourceUrl))
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"<i>Source: <a href='{quote.SourceUrl}' target='_blank'>Read more</a></i>");
            }

            return stringBuilder.ToString();
        }
    }
}