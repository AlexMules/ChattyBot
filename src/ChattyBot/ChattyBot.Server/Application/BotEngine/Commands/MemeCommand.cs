using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

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

        public async Task<string> ExecuteAsync(string? parameters = null)
        {
            var meme = await _memeRepo.GetRandomAsync();

            if (meme == null)
            {
                return "I ran out of memes. Try again later!";
            }

            return $"<img src='{meme.ImagePath}' alt='Meme' />";
        }
    }
}
