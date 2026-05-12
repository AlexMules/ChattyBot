using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class RandomCommand : IBotCommand
    {
        public string CommandTrigger => "/random";
        public string Description => "Generates a random integer between Min and Max.";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse("Provide Min and Max values! Example: /random 1 100", MessageType.Text);
            }

            var parts = parameters.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return new BotResponse("I need both a Min and a Max value! Example: /random -5 5", MessageType.Text);
            }

            if (!int.TryParse(parts[0], out int min) || !int.TryParse(parts[1], out int max))
            {
                return new BotResponse("Please use valid integers for Min and Max!", MessageType.Text);
            }

            if (min > max)
            {
                return new BotResponse($"Invalid Range: Min ({min}) cannot be greater than Max ({max}). Please check the order!", MessageType.Text);
            }

            int result = RandomGenerator.GetNext(min, max);

            return new BotResponse($"{result}", MessageType.Text);
        }
    }
}
