using ChattyBot.Server.Application.BotEngine;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Shared.Contracts.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using System.Text.Json;

namespace ChattyBot.Server.Commands
{
    public class VideoGameCommand : IBotCommand
    {
        private readonly IVideoGameRepository _repository;

        public string CommandTrigger => "/videogame";
        public string Description => "Get a random game recommendation!|Options: -fps, -rpg, -action-adventure, -simulation";

        public VideoGameCommand(IVideoGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            VideoGame? game;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                game = await _repository.GetRandomAsync();
            }
            else
            {
                GameCategory? category = parameters.ToLower().Trim() switch
                {
                    "-fps" => GameCategory.FPS,
                    "-rpg" => GameCategory.RPG,
                    "-action-adventure" => GameCategory.ActionAdventure,
                    "-simulation" => GameCategory.Simulation,
                    _ => null
                };

                if (category == null)
                {
                    return new BotResponse(
                        "I don't recognize that category. Try: -fps, -rpg, -action-adventure or -simulation.",
                        MessageType.Text);
                }

                game = await _repository.GetRandomByCategoryAsync(category.Value);
            }

            if (game == null)
            {
                return new BotResponse("No games found in this category. Try again later!", MessageType.Text);
            }

            var payload = new
            {
                game.Title,
                game.Description,
                game.ImagePath
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.Videogame);
        }
    }
}