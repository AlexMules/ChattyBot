using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChattyBot.Server.Infrastructure.Persistence.Context
{
    public class ChattyBotDbContext : DbContext
    {
        public ChattyBotDbContext(DbContextOptions<ChattyBotDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Version)
                .IsRowVersion()
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
