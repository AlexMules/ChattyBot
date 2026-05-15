using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class DiceCommand : IBotCommand
    {
        public string CommandTrigger => "/dice";
        public string Description => "Rolls two 6-sided dice with a cool animation.";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var payload = new
            {
                Die1 = RandomGenerator.GetNext(1, 6),
                Die2 = RandomGenerator.GetNext(1, 6)
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.Dice);
        }
    }
}
