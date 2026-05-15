using ChattyBot.Shared.Contracts.Enums;

namespace ChattyBot.Server.Application.BotEngine
{
    public class BotEngine : IBotEngine
    {
        private readonly IEnumerable<IBotCommand> _commands;
        private readonly BotCommandContext _context;

        public BotEngine(IEnumerable<IBotCommand> commands, BotCommandContext context)
        {
            _commands = commands;
            _context = context;
        }

        public static BotResponse GetWelcomeMessage(string username = "User")
        {
            string welcomeMessage = $"Hi there, {username}! I'm ChattyBot, your personal command-driven assistant.\n\n" +
                          "Everything I do is powered by slash commands. To discover my full range of features, just type /help.\n\n" +
                          "Let's get started!";

            return new BotResponse(welcomeMessage, MessageType.Text);
        }

        public async Task<BotResponse> ResolveAndExecuteAsync(string input, string username)
        {
            _context.Username = username;

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