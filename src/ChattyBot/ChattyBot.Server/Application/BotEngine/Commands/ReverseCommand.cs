using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class ReverseCommand : IBotCommand
    {
        public string CommandTrigger => "/reverse";
        public string Description => "Reverses the provided text.";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse
                (
                    "Please provide some text to reverse. Example: /reverse [text]",
                    MessageType.Text
                );
            }

            char[] charArray = parameters.ToCharArray();
            Array.Reverse(charArray);
            string reversedText = new string(charArray);

            return new BotResponse(reversedText, MessageType.Text);
        }
    }
}
