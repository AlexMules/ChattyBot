namespace ChattyBot.Server.Application.BotEngine
{
    public class BotEngine
    {
        private readonly IEnumerable<IBotCommand> _commands;

        public BotEngine(IEnumerable<IBotCommand> commands)
        {
            _commands = commands;
        }

        public async Task<string> ResolveAndExecuteAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please enter a command. Type /help to see what I can do!";
            }

            input = input.Trim();

            if (!input.StartsWith("/"))
            {
                return "Invalid format! All commands must start with a '/'. Example: /joke";
            }

            // parsing command and parameters
            var parts = input.Split(' ', 2);
            var trigger = parts[0].ToLower();
            var parameters = parts.Length > 1 ? parts[1] : null;

            var command = _commands.FirstOrDefault(c => c.CommandTrigger == trigger);

            if (command == null)
            {
                return $"I don't recognize the command '{trigger}'. Type /help to see the full list!";
            }

            return await command.ExecuteAsync(parameters);
        }
    }
}
