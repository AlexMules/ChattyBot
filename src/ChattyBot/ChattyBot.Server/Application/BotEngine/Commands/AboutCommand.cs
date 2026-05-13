namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class AboutCommand : IBotCommand
    {
        public string CommandTrigger => "/about";
        public string Description => "I will tell you more about who I am and how you can use my features!";

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var welcomeMessage = BotEngine.GetWelcomeMessage();

            return await Task.FromResult(welcomeMessage);
        }
    }
}
