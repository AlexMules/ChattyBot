using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class MorseCommand : IBotCommand
    {
        public string CommandTrigger => "/morse";
        public string Description => "Translates text to Morse code.";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse("Please provide some text. Usage: /morse [text]", MessageType.Text);
            }

            string translated = MorseTranslator.ToMorse(parameters);

            return new BotResponse(translated, MessageType.Text);
        }
    }
}
