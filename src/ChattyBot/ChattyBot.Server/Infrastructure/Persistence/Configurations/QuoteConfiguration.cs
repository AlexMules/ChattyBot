using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.HasData(
                new Quote { Id = 1, Text = "Two things are infinite: the universe and human stupidity; and I'm not sure about the universe.", Author = "Albert Einstein", SourceUrl = "https://en.wikipedia.org/wiki/Albert_Einstein" },
                new Quote { Id = 2, Text = "A room without books is like a body without a soul.", Author = "Marcus Tullius Cicero", SourceUrl = "https://en.wikipedia.org/wiki/Cicero" },
                new Quote { Id = 3, Text = "Be the change that you wish to see in the world.", Author = "Mahatma Gandhi", SourceUrl = "https://en.wikipedia.org/wiki/Mahatma_Gandhi" },
                new Quote { Id = 4, Text = "If you tell the truth, you don't have to remember anything.", Author = "Mark Twain", SourceUrl = "https://en.wikipedia.org/wiki/Mark_Twain" },
                new Quote { Id = 5, Text = "To live is the rarest thing in the world. Most people exist, that is all.", Author = "Oscar Wilde", SourceUrl = "https://en.wikipedia.org/wiki/Oscar_Wilde" },
                new Quote { Id = 6, Text = "Darkness cannot drive out darkness: only light can do that. Hate cannot drive out hate: only love can do that.", Author = "Martin Luther King Jr.", SourceUrl = "https://en.wikipedia.org/wiki/Martin_Luther_King_Jr." },
                new Quote { Id = 7, Text = "Without music, life would be a mistake.", Author = "Friedrich Nietzsche", SourceUrl = "https://en.wikipedia.org/wiki/Friedrich_Nietzsche" },
                new Quote { Id = 8, Text = "It is better to be hated for what you are than to be loved for what you are not.", Author = "André Gide", SourceUrl = "https://en.wikipedia.org/wiki/André_Gide" },
                new Quote { Id = 9, Text = "Doing nothing is very hard to do. You never know when you’re finished.", Author = "Leslie Nielsen", SourceUrl = "https://en.wikipedia.org/wiki/Leslie_Nielsen" },
                new Quote { Id = 10, Text = "The price of greatness is responsibility.", Author = "Winston Churchill", SourceUrl = "https://en.wikipedia.org/wiki/Winston_Churchill" }
            );
        }
    }
}