using System.Text;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class HelpCommand : IBotCommand
    {
        private readonly IServiceProvider _serviceProvider;

        public string CommandTrigger => "/help";
        public string Description => "Lists all available commands and their descriptions.";

        public HelpCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<string> ExecuteAsync(string? parameters = null)
        {
            var allCommands = _serviceProvider.GetServices<IBotCommand>()
                                .OrderBy(c => c.CommandTrigger);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Here are the commands I can help you with:");
            stringBuilder.AppendLine();

            foreach (var command in allCommands)
            {
                stringBuilder.AppendLine($"<b>{command.CommandTrigger}</b> - {command.Description}");
            }

            return Task.FromResult(stringBuilder.ToString());
        }
    }
}
