using ChattyBot.Server.Application.BotEngine;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Commands
{
    public class MusicCommand : IBotCommand
    {
        private readonly ISongRepository _repository;

        public string CommandTrigger => "/music";
        public string Description => "Get a random song recommendation!|Options: -rock, -pop, -jazz, -rap";

        public MusicCommand(ISongRepository repository)
        {
            _repository = repository;
        }

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            Song? song;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                song = await _repository.GetRandomAsync();
            }
            else
            {
                MusicCategory? category = parameters.ToLower().Trim() switch
                {
                    "-rock" => MusicCategory.Rock,
                    "-pop" => MusicCategory.Pop,
                    "-jazz" => MusicCategory.Jazz,
                    "-rap" => MusicCategory.Rap,
                    _ => null
                };

                if (category == null)
                {
                    return new BotResponse("I don't recognize that genre. Try: -rock, -pop, -jazz or -rap.", MessageType.Text);
                }

                song = await _repository.GetRandomByCategoryAsync(category.Value);
            }

            if (song == null)
            {
                return new BotResponse("I couldn't find any songs in this category. Try again later!", MessageType.Text);
            }

            var payload = new
            {
                song.Title,
                song.Artist,
                song.Description,
                song.SongPath
            };

            return new BotResponse(JsonSerializer.Serialize(payload), MessageType.Music);
        }
    }
}