using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class ChooseCommand : IBotCommand
    {
        public string CommandTrigger => "/choose";
        public string Description => "Picks a random item from a comma-separated list.";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse("Please provide options separated by commas! Example: /choose Pizza, Burger, Sushi", MessageType.Text);
            }

            var options = parameters.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(opt => opt.Trim())
                                      .Where(opt => !string.IsNullOrWhiteSpace(opt))
                                      .ToList();

            if (options.Count < 2)
            {
                return new BotResponse("I need at least two options to make a choice!", MessageType.Text);
            }

            int randomIndex = RandomGenerator.GetNext(0, options.Count - 1);
            string selectedOption = options[randomIndex];

            return new BotResponse($"I choose: {selectedOption}", MessageType.Text);
        }
    }
}
