using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.Json;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class MemeCommand : IBotCommand
    {
        private readonly IMemeRepository _memeRepo;

        public string CommandTrigger => "/meme";
        public string Description => "Get a random funny meme!";

        public MemeCommand(IMemeRepository memeRepo)
        {
            _memeRepo = memeRepo;
        }

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var meme = await _memeRepo.GetRandomAsync();

            if (meme == null)
            {
                return new BotResponse("I ran out of memes. Try again later!", MessageType.Text);
            }

            var payload = new
            {
                Url = meme.ImagePath
            };

            string jsonContent = JsonSerializer.Serialize(payload);

            return new BotResponse(jsonContent, MessageType.Image);
        }
    }
}
