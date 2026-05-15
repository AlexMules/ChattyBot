using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.RegularExpressions;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class MorseCommand : IBotCommand
    {
        public string CommandTrigger => "/morse";
        public string Description => "Translates text to Morse code.";
        private static readonly Regex InvalidMorseCharsRegex = new(@"[^A-Za-z0-9\s]", RegexOptions.Compiled);

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse("Please provide some text. Usage: /morse text", MessageType.Text);
            }

            if (InvalidMorseCharsRegex.IsMatch(parameters))
            {
                return new BotResponse("Error: Morse translation only supports letters (A-Z, a-z), numbers (0-9), and spaces!", MessageType.Text);
            }

            string translated = MorseTranslator.ToMorse(parameters);

            return new BotResponse(translated, MessageType.Text);
        }
    }
}
