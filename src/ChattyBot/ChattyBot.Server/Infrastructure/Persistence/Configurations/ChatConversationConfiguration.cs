using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
    {
        public void Configure(EntityTypeBuilder<ChatConversation> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasOne(c => c.User)
                .WithMany() 
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // cascade delete conversations when a user is deleted
        }
    }
}