using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine
{
    public class BotEngine
    {
        private readonly IEnumerable<IBotCommand> _commands;

        public BotEngine(IEnumerable<IBotCommand> commands)
        {
            _commands = commands;
        }

        public async Task<BotResponse> ResolveAndExecuteAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new BotResponse("Please enter a command. Type /help to see what I can do!", MessageType.Text);
            }

            input = input.Trim();

            if (!input.StartsWith("/"))
            {
                return new BotResponse("Invalid format! All commands must start with a '/'. Example: /joke", MessageType.Text);
            }

            var parts = input.Split(' ', 2);
            var trigger = parts[0].ToLower();
            var parameters = parts.Length > 1 ? parts[1] : null;

            var command = _commands.FirstOrDefault(c => c.CommandTrigger == trigger);

            if (command == null)
            {
                return new BotResponse($"I don't recognize the command '{trigger}'. Type /help to see the full list!", MessageType.Text);
            }

            return await command.ExecuteAsync(parameters);
        }
    }
}