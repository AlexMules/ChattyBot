using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChattyBot.Server.Infrastructure.Persistence.Context
{
    public class ChattyBotDbContext : DbContext
    {
        public ChattyBotDbContext(DbContextOptions<ChattyBotDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Joke> Jokes => Set<Joke>();
        public DbSet<FunFact> FunFacts => Set<FunFact>();
        public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<Meme> Memes => Set<Meme>();
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<TriviaQuestion> TriviaQuestions => Set<TriviaQuestion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChattyBotDbContext).Assembly);
        }
    }
}