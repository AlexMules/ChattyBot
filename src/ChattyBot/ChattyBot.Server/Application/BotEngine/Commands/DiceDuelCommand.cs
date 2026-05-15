using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class DiceDuelCommand : IBotCommand
    {
        public string CommandTrigger => "/dice-duel";
        public string Description => "Duel with ChattyBot! Who rolls higher wins!";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var payload = new
            {
                UserRoll = RandomGenerator.GetNext(1, 6),
                BotRoll = RandomGenerator.GetNext(1, 6)
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.DiceDuel);
        }
    }
}
