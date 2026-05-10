using ChattyBot.Server.Infrastructure.Persistence.Interfaces;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class JokeCommand : IBotCommand
    {
        private readonly IJokeRepository _jokeRepo;
        public string CommandTrigger => "/joke";
        public string Description => "Tells a random joke to brighten your day.";

        public JokeCommand(IJokeRepository jokeRepo)
        {
            _jokeRepo = jokeRepo;
        }

        public async Task<string> ExecuteAsync(string? parameters = null)
        {
            var joke = await _jokeRepo.GetRandomAsync();

            return joke?.Content ?? "I'm fresh out of jokes today. Try again later!";
        }
    }
}
