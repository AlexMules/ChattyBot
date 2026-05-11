using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class DiceCommand : IBotCommand
    {
        public string CommandTrigger => "/dice";
        public string Description => "Rolls two 6-sided dice with a cool animation.";

        private readonly Random _random = new();

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var payload = new
            {
                Die1 = _random.Next(1, 7),
                Die2 = _random.Next(1, 7)
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.Dice);
        }
    }
}
