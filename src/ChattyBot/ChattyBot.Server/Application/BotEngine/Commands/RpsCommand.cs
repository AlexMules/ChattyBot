using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class RpsCommand : IBotCommand
    {
        public string CommandTrigger => "/rps";
        public string Description => "Play Rock Paper Scissors with ChattyBot!";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse("Choose your weapon: /rps -rock, /rps -paper, or /rps -scissors !", MessageType.Text);
            }

            string userChoice = parameters.ToLower().Replace("-", "").Trim();
            string[] validChoices = { "rock", "paper", "scissors" };

            if (!validChoices.Contains(userChoice))
            {
                return new BotResponse("Invalid choice! Use -rock, -paper, or -scissors.", MessageType.Text);
            }

            string botChoice = validChoices[RandomGenerator.GetNext(0, 2)];

            var payload = new { UserChoice = userChoice, BotChoice = botChoice };

            return new BotResponse(JsonSerializer.Serialize(payload), MessageType.RpsDuel);
        }
    }
}
