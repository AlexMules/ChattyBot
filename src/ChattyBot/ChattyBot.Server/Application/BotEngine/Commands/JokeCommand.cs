using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.Enums;

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

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            var joke = await _jokeRepo.GetRandomAsync();

            if (joke == null)
            {
                return new BotResponse
                (
                    "I'm fresh out of jokes today. Try again later!",
                    MessageType.Text
                );
            }
            return new BotResponse(joke.Content, MessageType.Text);
        }
    }
}
