using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class CoinFlipCommand : IBotCommand
    {
        public string CommandTrigger => "/coinflip";
        public string Description => "Flip a coin!";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            int flip = RandomGenerator.GetNext(0, 1);
            string result = flip == 0 ? "Heads" : "Tails";

            var payload = new { Result = result };
            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.CoinFlip);
        }
    }
}
