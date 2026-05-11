using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class CoinFlipCommand : IBotCommand
    {
        public string CommandTrigger => "/coinflip";
        public string Description => "Flip a coin!";

        private readonly Random _random = new();

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            // 0 = Heads, 1 = Tails
            string result = _random.Next(0, 2) == 0 ? "Heads" : "Tails";

            var payload = new { Result = result };
            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.CoinFlip);
        }
    }
}
