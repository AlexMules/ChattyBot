using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

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

        public Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var commandsList = _serviceProvider.GetServices<IBotCommand>()
                                .OrderBy(c => c.CommandTrigger)
                                .Select(c => new
                                {
                                    Trigger = c.CommandTrigger,
                                    Description = c.Description
                                })
                                .ToList();

            string jsonContent = JsonSerializer.Serialize(commandsList);

            return Task.FromResult(new BotResponse(jsonContent, MessageType.Help));
        }
    }
}
