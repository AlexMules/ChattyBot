using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class MemeConfiguration : IEntityTypeConfiguration<Meme>
    {
        public void Configure(EntityTypeBuilder<Meme> builder)
        {
            builder.HasData(
                new Meme { Id = 1, ImagePath = "/images/memes/meme1.jpg" },
                new Meme { Id = 2, ImagePath = "/images/memes/meme2.jpg" },
                new Meme { Id = 3, ImagePath = "/images/memes/meme3.jpg" },
                new Meme { Id = 4, ImagePath = "/images/memes/meme4.jpg" },
                new Meme { Id = 5, ImagePath = "/images/memes/meme5.jpg" },
                new Meme { Id = 6, ImagePath = "/images/memes/meme6.jpg" },
                new Meme { Id = 7, ImagePath = "/images/memes/meme7.jpg" },
                new Meme { Id = 8, ImagePath = "/images/memes/meme8.jpg" },
                new Meme { Id = 9, ImagePath = "/images/memes/meme9.jpg" },
                new Meme { Id = 10, ImagePath = "/images/memes/meme10.jpg" }
            );
        }
    }
}