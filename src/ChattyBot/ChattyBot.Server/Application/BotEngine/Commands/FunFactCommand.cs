using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

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

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var fact = await _funFactRepo.GetRandomAsync();

            if (fact == null)
            {
                return new BotResponse("I couldn't find any interesting facts right now. Try again later!", MessageType.Text);
            }

            var payload = new
            {
                Text = fact.Content,
                SourceUrl = fact.SourceUrl
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.FunFact);
        }
    }
}