using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using System.Text;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class FunFactCommand : IBotCommand
    {
        private readonly IFunFactRepository _funFactRepo;

        public string CommandTrigger => "/funfact";
        public string Description => "Did you know? Get an interesting fact and its source!";

        public FunFactCommand(IFunFactRepository funFactRepo)
        {
            _funFactRepo = funFactRepo;
        }

        public async Task<string> ExecuteAsync(string? parameters = null)
        {
            var fact = await _funFactRepo.GetRandomAsync();

            if (fact == null)
            {
                return "I couldn't find any interesting facts right now. Try again later!";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(fact.Content);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"<i>Source: <a href='{fact.SourceUrl}' target='_blank'>Click here</a></i>");

            return stringBuilder.ToString();
        }
    }
}