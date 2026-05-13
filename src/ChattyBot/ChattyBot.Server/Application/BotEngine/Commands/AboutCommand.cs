namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class AboutCommand : IBotCommand
    {
        private readonly BotCommandContext _context;

        public string CommandTrigger => "/about";
        public string Description => "I will tell you more about who I am and how you can use my features!";

        public AboutCommand(BotCommandContext context)
        {
            _context = context;
        }

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var username = _context.Username;

            var welcomeMessage = BotEngine.GetWelcomeMessage(username);

            return await Task.FromResult(welcomeMessage);
        }
    }
}
