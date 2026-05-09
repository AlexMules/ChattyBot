using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class SongConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder.HasData(
                // rock songs
                new Song
                {
                    Id = 1,
                    Title = "Bohemian Rhapsody",
                    Artist = "Queen",
                    Category = MusicCategory.Rock,
                    Description = "A six-minute suite notable for its lack of a refraining chorus, consisting of several sections: an intro, a ballad segment, an operatic passage, a hard rock part, and a reflective coda. It is one of the few progressive rock songs to become a global mainstream phenomenon.",
                    SongPath = "https://www.youtube.com/watch?v=fJ9rUzIMcZQ"
                },
                new Song
                {
                    Id = 2,
                    Title = "Back in Black",
                    Artist = "AC/DC",
                    Category = MusicCategory.Rock,
                    Description = "Released in 1980 as a tribute to their late singer Bon Scott, this album marked the debut of Brian Johnson. It is one of the best-selling albums in history, featuring some of the most iconic guitar riffs ever recorded in hard rock.",
                    SongPath = "https://www.youtube.com/watch?v=pAgnJDJN4VA"
                },
                new Song
                {
                    Id = 3,
                    Title = "In the End",
                    Artist = "Linkin Park",
                    Category = MusicCategory.Rock,
                    Description = "A defining song of the nu-metal era, featured on their debut album Hybrid Theory. The track is famous for the interplay between Chester Bennington's melodic vocals and Mike Shinoda's rap verses, reflecting on themes of failure and time.",
                    SongPath = "https://www.youtube.com/watch?v=eVTXPUF4Oz4"
                },
                new Song
                {
                    Id = 4,
                    Title = "Nothing Else Matters",
                    Artist = "Metallica",
                    Category = MusicCategory.Rock,
                    Description = "A legendary power ballad released in 1991. Recognized as one of Metallica's most popular songs, it showcases a softer, more melodic side of the heavy metal giants and has become a staple in their live performances worldwide.",
                    SongPath = "https://www.youtube.com/watch?v=tAGnKpE4NCI"
                },

                // pop songs
                new Song
                {
                    Id = 5,
                    Title = "Billie Jean",
                    Artist = "Michael Jackson",
                    Category = MusicCategory.Pop,
                    Description = "A revolutionary blend of post-disco, R&B, and funk. Released in 1983, it features one of the most recognizable basslines in music history and tells the story of a woman claiming the narrator is the father of her child.",
                    SongPath = "https://www.youtube.com/watch?v=Zi_XLOBDo_Y"
                },
                new Song
                {
                    Id = 6,
                    Title = "Shake It Off",
                    Artist = "Taylor Swift",
                    Category = MusicCategory.Pop,
                    Description = "An uptempo dance-pop anthem released in 2014, marking Swift's full transition to pop music. The lyrics express indifference toward detractors and negative media scrutiny, set to a catchy drum beat and saxophone line.",
                    SongPath = "https://www.youtube.com/watch?v=nfWlot6h_JM"
                },
                new Song
                {
                    Id = 7,
                    Title = "As It Was",
                    Artist = "Harry Styles",
                    Category = MusicCategory.Pop,
                    Description = "A synth-pop and new wave track released in 2022. The song features a nostalgic 80s-inspired sound and explores themes of personal transition and changing relationships, becoming a record-breaking global hit.",
                    SongPath = "https://www.youtube.com/watch?v=H5v3kku4y6Q"
                },
                new Song
                {
                    Id = 8,
                    Title = "Levitating",
                    Artist = "Dua Lipa",
                    Category = MusicCategory.Pop,
                    Description = "A high-energy track that combines disco and dance-pop influences. Featured on the album Future Nostalgia, it uses outer space metaphors to describe the feeling of falling in love and became a defining hit of the early 2020s.",
                    SongPath = "https://www.youtube.com/watch?v=TUVcZfQe-Kw"
                },

                // jazz songs
                new Song
                {
                    Id = 9,
                    Title = "So What",
                    Artist = "Miles Davis",
                    Category = MusicCategory.Jazz,
                    Description = "A 1959 masterpiece and the quintessential example of modal jazz. Using the Dorian mode, the piece relies on melodic improvisation rather than complex chord changes, setting a new direction for modern jazz music.",
                    SongPath = "https://www.youtube.com/watch?v=ylXk1LBvIqU"
                },
                new Song
                {
                    Id = 10,
                    Title = "What a Wonderful World",
                    Artist = "Louis Armstrong",
                    Category = MusicCategory.Jazz,
                    Description = "First recorded in 1967, this song is a hopeful anthem about the beauty of the world and the future of humanity. Armstrong's gravelly, soulful voice turned the track into a timeless classic recognized by all generations.",
                    SongPath = "https://www.youtube.com/watch?v=rBrd_3VMC3c"
                },
                new Song
                {
                    Id = 11,
                    Title = "Hello, Dolly!",
                    Artist = "Louis Armstrong",
                    Category = MusicCategory.Jazz,
                    Description = "The title song from the 1964 musical of the same name. Armstrong's rendition was a massive success, famously displacing the Beatles from the top of the charts and earning him multiple Grammy Awards.",
                    SongPath = "https://www.youtube.com/watch?v=Kx2AYFvwxKY"
                },

                // rap songs
                new Song
                {
                    Id = 12,
                    Title = "Rap God",
                    Artist = "Eminem",
                    Category = MusicCategory.Rap,
                    Description = "A lyrical powerhouse released in 2013, famous for its speed and technical complexity. With 1,560 words, it entered the Guinness World Records for the most words in a hit single, showcasing Eminem's immense rapping skill.",
                    SongPath = "https://www.youtube.com/watch?v=XbGs_qK2PQA"
                },
                new Song
                {
                    Id = 13,
                    Title = "Humble",
                    Artist = "Kendrick Lamar",
                    Category = MusicCategory.Rap,
                    Description = "A hard-hitting call to humility released in 2017. Produced by Mike WiLL Made-It, the song features a minimalist, piano-driven beat and sharp lyrics that critiques modern ego and social media culture.",
                    SongPath = "https://www.youtube.com/watch?v=tvTRZJ-4EyI"
                },
                new Song
                {
                    Id = 14,
                    Title = "Juicy",
                    Artist = "The Notorious B.I.G.",
                    Category = MusicCategory.Rap,
                    Description = "Widely considered one of the greatest hip-hop tracks ever made. Released in 1994, it tells Biggie's 'rags-to-riches' story, sampling Mtume's 'Juicy Fruit' to create a smooth, soulful, and triumphant anthem.",
                    SongPath = "https://www.youtube.com/watch?v=_JZom_gVfuw"
                },
                new Song
                {
                    Id = 15,
                    Title = "Hit 'Em Up",
                    Artist = "2Pac",
                    Category = MusicCategory.Rap,
                    Description = "Perhaps the most famous diss track in history, recorded during the peak of the East Coast-West Coast rivalry. Released in 1996, the song contains vicious insults aimed primarily at The Notorious B.I.G. and Bad Boy Records.",
                    SongPath = "https://www.youtube.com/watch?v=41qC3w3UUkU"
                }
            );
        }
    }
}