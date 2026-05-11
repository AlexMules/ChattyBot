using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class DiceDuelCommand : IBotCommand
    {
        public string CommandTrigger => "/dice-duel";
        public string Description => "Duel with ChattyBot! Who rolls higher wins!";

        private readonly Random _random = new();

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var payload = new
            {
                UserRoll = _random.Next(1, 7),
                BotRoll = _random.Next(1, 7)
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.DiceDuel);
        }
    }
}
