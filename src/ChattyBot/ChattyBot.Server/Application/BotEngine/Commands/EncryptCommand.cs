using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class EncryptCommand : IBotCommand
    {
        public string CommandTrigger => "/encrypt";
        public string Description => "Encrypts a message using the Caesar Cipher (Shift = 3).";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse
                (
                    "Please provide some text to encrypt. Usage: /encrypt text",
                    MessageType.Text
                );
            }

            string encryptedResult = CaesarCipher.Encrypt(parameters, 3);

            return new BotResponse(encryptedResult, MessageType.Text);
        }
    }
}
