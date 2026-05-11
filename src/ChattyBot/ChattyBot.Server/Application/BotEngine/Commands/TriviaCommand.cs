using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class TriviaCommand : IBotCommand
    {
        private readonly ITriviaService _triviaService;

        public string CommandTrigger => "/trivia";
        public string Description => "Test your knowledge!|Options: -football, -gaming, -science, -history";

        public TriviaCommand(ITriviaService triviaService) => _triviaService = triviaService;

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            TriviaCategory? category = null;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                category = parameters.ToLower().Trim() switch
                {
                    "-football" => TriviaCategory.Football,
                    "-gaming" => TriviaCategory.Gaming,
                    "-science" => TriviaCategory.Science,
                    "-history" => TriviaCategory.History,
                    _ => null
                };

                if (category == null)
                {
                    return new BotResponse("I don't recognize that category. Try: -football, -gaming, -science or -history", MessageType.Text);
                }
            }

            var dto = await _triviaService.GetQuestionAsync(category);

            if (dto == null)
            {
                return new BotResponse("No questions found for this category. Try again later!", MessageType.Text);
            }

            return new BotResponse(JsonSerializer.Serialize(dto), MessageType.Trivia);
        }
    }
}
