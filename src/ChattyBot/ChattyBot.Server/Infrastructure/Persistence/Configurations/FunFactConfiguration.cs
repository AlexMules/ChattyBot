using ChattyBot.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class FunFactConfiguration : IEntityTypeConfiguration<FunFact>
    {
        public void Configure(EntityTypeBuilder<FunFact> builder)
        {
            builder.HasData(
                new FunFact
                {
                    Id = 1,
                    Content = "Honey never spoils.\nArchaeologists have found edible honey in millennia-old Egyptian tombs!",
                    SourceUrl = "https://www.smithsonianmag.com/science-nature/the-science-behind-honeys-eternal-shelf-life-1218690/"
                },
                new FunFact
                {
                    Id = 2,
                    Content = "Venus is the only planet that rotates clockwise.\nIt spins in the opposite direction to most other planets in our solar system.",
                    SourceUrl = "https://science.nasa.gov/venus/facts/"
                },
                new FunFact
                {
                    Id = 3,
                    Content = "The first 'computer bug' was an actual insect.\nIn 1947, engineers found a moth trapped in a relay of the Harvard Mark II computer.",
                    SourceUrl = "https://www.computerhistory.org/tdih/september/9/"
                },
                new FunFact
                {
                    Id = 4,
                    Content = "The shortest war in history lasted only 38 minutes.\nThe Anglo-Zanzibar War occurred on August 27, 1896, between the UK and Zanzibar.",
                    SourceUrl = "https://www.historic-uk.com/HistoryUK/HistoryofBritain/The-Shortest-War-in-History/"
                },
                new FunFact
                {
                    Id = 5,
                    Content = "Octopuses have three hearts.\nTwo pump blood to the gills, while the third pumps it to the rest of the body.",
                    SourceUrl = "https://www.newscientist.com/question/many-hearts-octopus/"
                },
                new FunFact
                {
                    Id = 6,
                    Content = "France is the country with the most time zones in the world.\nIncluding its overseas territories, it covers a total of 12 different time zones.",
                    SourceUrl = "https://www.timeanddate.com/time/country-with-the-most-time-zones.html"
                },
                new FunFact
                {
                    Id = 7,
                    Content = "Sloths can hold their breath longer than dolphins.\nBy slowing their heart rate, they can stay underwater for up to 40 minutes.",
                    SourceUrl = "https://www.bbc.co.uk/bitesize/articles/zqk4qyc"
                },
                new FunFact
                {
                    Id = 8,
                    Content = "Koalas have fingerprints that are almost identical to humans.\nEven under a microscope, it’s difficult to distinguish them from human prints.",
                    SourceUrl = "https://www.pbs.org/wgbh/nova/article/koala-fingerprints/"
                },
                new FunFact
                {
                    Id = 9,
                    Content = "It rains diamonds on Saturn and Jupiter.\nExtreme pressure turns atmospheric methane into soot, then graphite, and finally diamonds.",
                    SourceUrl = "https://www.bbc.com/news/science-environment-24477667"
                },
                new FunFact
                {
                    Id = 10,
                    Content = "The 'smell of rain' is actually caused by bacteria and plants.\nIt’s called petrichor and is released from the soil when hit by raindrops.",
                    SourceUrl = "https://www.acs.org/content/dam/acsorg/education/students/highschool/chemistryclubs/infographics/petrichor-the-smell-of-rain.pdf"
                }
            );
        }
    }
}