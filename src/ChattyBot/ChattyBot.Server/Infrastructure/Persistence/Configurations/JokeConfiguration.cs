using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class JokeConfiguration : IEntityTypeConfiguration<Joke>
    {
        public void Configure(EntityTypeBuilder<Joke> builder)
        {
            builder.HasData(
                new Joke { Id = 1, Content = "What did the shark say when he ate the clownfish?\nThis tastes a little funny." },
                new Joke { Id = 2, Content = "Why couldn’t the bad sailor learn the alphabet?\nBecause he always got lost at “C.”" },
                new Joke { Id = 3, Content = "What did one ocean say to the other ocean?\nNothing, they just waved." },
                new Joke { Id = 4, Content = "A Roman soldier walks into a bar, holds up two fingers and says:\n\"Five beers, please!\"" },
                new Joke { Id = 5, Content = "I'm on a seafood diet.\nI see food and I eat it." },
                new Joke { Id = 6, Content = "How do you throw a space party?\nYou planet." },
                new Joke { Id = 7, Content = "The numbers 19 and 20 got into a fight.\n21." },
                new Joke { Id = 8, Content = "I’d tell you a pizza joke …\nbut it’s probably too cheesy." },
                new Joke { Id = 9, Content = "My grandpa always said when one door closes, another one opens.\nSmart man but a horrible cabinet maker." },
                new Joke { Id = 10, Content = "How many programmers does it take to change a light bulb?\nNone. They can't do it, that's a hardware problem!" }
            );
        }
    }
}