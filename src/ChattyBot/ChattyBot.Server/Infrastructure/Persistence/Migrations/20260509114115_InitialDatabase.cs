using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChattyBot.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FunFacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext", nullable: false),
                    SourceUrl = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunFacts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Jokes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jokes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Memes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ImagePath = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "longtext", nullable: false),
                    Author = table.Column<string>(type: "longtext", nullable: false),
                    SourceUrl = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    Artist = table.Column<string>(type: "longtext", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    SongPath = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TriviaQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    QuestionText = table.Column<string>(type: "longtext", nullable: false),
                    Options = table.Column<string>(type: "longtext", nullable: false),
                    CorrectAnswerIndex = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaQuestions", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false),
                    Username = table.Column<string>(type: "longtext", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false),
                    AvatarPath = table.Column<string>(type: "longtext", nullable: false),
                    Version = table.Column<DateTime>(type: "timestamp", rowVersion: true, nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VideoGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    ImagePath = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGames", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "FunFacts",
                columns: new[] { "Id", "Content", "SourceUrl" },
                values: new object[,]
                {
                    { 1, "Honey never spoils.\nArchaeologists have found edible honey in millennia-old Egyptian tombs!", "https://www.smithsonianmag.com/science-nature/the-science-behind-honeys-eternal-shelf-life-1218690/" },
                    { 2, "Venus is the only planet that rotates clockwise.\nIt spins in the opposite direction to most other planets in our solar system.", "https://science.nasa.gov/venus/facts/" },
                    { 3, "The first 'computer bug' was an actual insect.\nIn 1947, engineers found a moth trapped in a relay of the Harvard Mark II computer.", "https://www.computerhistory.org/tdih/september/9/" },
                    { 4, "The shortest war in history lasted only 38 minutes.\nThe Anglo-Zanzibar War occurred on August 27, 1896, between the UK and Zanzibar.", "https://www.historic-uk.com/HistoryUK/HistoryofBritain/The-Shortest-War-in-History/" },
                    { 5, "Octopuses have three hearts.\nTwo pump blood to the gills, while the third pumps it to the rest of the body.", "https://www.newscientist.com/question/many-hearts-octopus/" },
                    { 6, "France is the country with the most time zones in the world.\nIncluding its overseas territories, it covers a total of 12 different time zones.", "https://www.timeanddate.com/time/country-with-the-most-time-zones.html" },
                    { 7, "Sloths can hold their breath longer than dolphins.\nBy slowing their heart rate, they can stay underwater for up to 40 minutes.", "https://www.bbc.co.uk/bitesize/articles/zqk4qyc" },
                    { 8, "Koalas have fingerprints that are almost identical to humans.\nEven under a microscope, it’s difficult to distinguish them from human prints.", "https://www.pbs.org/wgbh/nova/article/koala-fingerprints/" },
                    { 9, "It rains diamonds on Saturn and Jupiter.\nExtreme pressure turns atmospheric methane into soot, then graphite, and finally diamonds.", "https://www.bbc.com/news/science-environment-24477667" },
                    { 10, "The 'smell of rain' is actually caused by bacteria and plants.\nIt’s called petrichor and is released from the soil when hit by raindrops.", "https://www.acs.org/content/dam/acsorg/education/students/highschool/chemistryclubs/infographics/petrichor-the-smell-of-rain.pdf" }
                });

            migrationBuilder.InsertData(
                table: "Jokes",
                columns: new[] { "Id", "Content" },
                values: new object[,]
                {
                    { 1, "What did the shark say when he ate the clownfish?\nThis tastes a little funny." },
                    { 2, "Why couldn’t the bad sailor learn the alphabet?\nBecause he always got lost at “C.”" },
                    { 3, "What did one ocean say to the other ocean?\nNothing, they just waved." },
                    { 4, "A Roman soldier walks into a bar, holds up two fingers and says:\n\"Five beers, please!\"" },
                    { 5, "I'm on a seafood diet.\nI see food and I eat it." },
                    { 6, "How do you throw a space party?\nYou planet." },
                    { 7, "The numbers 19 and 20 got into a fight.\n21." },
                    { 8, "I’d tell you a pizza joke …\nbut it’s probably too cheesy." },
                    { 9, "My grandpa always said when one door closes, another one opens.\nSmart man but a horrible cabinet maker." },
                    { 10, "How many programmers does it take to change a light bulb?\nNone. They can't do it, that's a hardware problem!" }
                });

            migrationBuilder.InsertData(
                table: "Memes",
                columns: new[] { "Id", "ImagePath" },
                values: new object[,]
                {
                    { 1, "/images/memes/meme1.jpg" },
                    { 2, "/images/memes/meme2.jpg" },
                    { 3, "/images/memes/meme3.jpg" },
                    { 4, "/images/memes/meme4.jpg" },
                    { 5, "/images/memes/meme5.jpg" },
                    { 6, "/images/memes/meme6.jpg" },
                    { 7, "/images/memes/meme7.jpg" },
                    { 8, "/images/memes/meme8.jpg" },
                    { 9, "/images/memes/meme9.jpg" },
                    { 10, "/images/memes/meme10.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Quotes",
                columns: new[] { "Id", "Author", "SourceUrl", "Text" },
                values: new object[,]
                {
                    { 1, "Albert Einstein", "https://en.wikipedia.org/wiki/Albert_Einstein", "Two things are infinite: the universe and human stupidity; and I'm not sure about the universe." },
                    { 2, "Marcus Tullius Cicero", "https://en.wikipedia.org/wiki/Cicero", "A room without books is like a body without a soul." },
                    { 3, "Mahatma Gandhi", "https://en.wikipedia.org/wiki/Mahatma_Gandhi", "Be the change that you wish to see in the world." },
                    { 4, "Mark Twain", "https://en.wikipedia.org/wiki/Mark_Twain", "If you tell the truth, you don't have to remember anything." },
                    { 5, "Oscar Wilde", "https://en.wikipedia.org/wiki/Oscar_Wilde", "To live is the rarest thing in the world. Most people exist, that is all." },
                    { 6, "Martin Luther King Jr.", "https://en.wikipedia.org/wiki/Martin_Luther_King_Jr.", "Darkness cannot drive out darkness: only light can do that. Hate cannot drive out hate: only love can do that." },
                    { 7, "Friedrich Nietzsche", "https://en.wikipedia.org/wiki/Friedrich_Nietzsche", "Without music, life would be a mistake." },
                    { 8, "André Gide", "https://en.wikipedia.org/wiki/André_Gide", "It is better to be hated for what you are than to be loved for what you are not." },
                    { 9, "Leslie Nielsen", "https://en.wikipedia.org/wiki/Leslie_Nielsen", "Doing nothing is very hard to do. You never know when you’re finished." },
                    { 10, "Winston Churchill", "https://en.wikipedia.org/wiki/Winston_Churchill", "The price of greatness is responsibility." }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "Artist", "Category", "Description", "SongPath", "Title" },
                values: new object[,]
                {
                    { 1, "Queen", 0, "A six-minute suite notable for its lack of a refraining chorus, consisting of several sections: an intro, a ballad segment, an operatic passage, a hard rock part, and a reflective coda. It is one of the few progressive rock songs to become a global mainstream phenomenon.", "https://www.youtube.com/watch?v=fJ9rUzIMcZQ", "Bohemian Rhapsody" },
                    { 2, "AC/DC", 0, "Released in 1980 as a tribute to their late singer Bon Scott, this album marked the debut of Brian Johnson. It is one of the best-selling albums in history, featuring some of the most iconic guitar riffs ever recorded in hard rock.", "https://www.youtube.com/watch?v=pAgnJDJN4VA", "Back in Black" },
                    { 3, "Linkin Park", 0, "A defining song of the nu-metal era, featured on their debut album Hybrid Theory. The track is famous for the interplay between Chester Bennington's melodic vocals and Mike Shinoda's rap verses, reflecting on themes of failure and time.", "https://www.youtube.com/watch?v=eVTXPUF4Oz4", "In the End" },
                    { 4, "Metallica", 0, "A legendary power ballad released in 1991. Recognized as one of Metallica's most popular songs, it showcases a softer, more melodic side of the heavy metal giants and has become a staple in their live performances worldwide.", "https://www.youtube.com/watch?v=tAGnKpE4NCI", "Nothing Else Matters" },
                    { 5, "Michael Jackson", 1, "A revolutionary blend of post-disco, R&B, and funk. Released in 1983, it features one of the most recognizable basslines in music history and tells the story of a woman claiming the narrator is the father of her child.", "https://www.youtube.com/watch?v=Zi_XLOBDo_Y", "Billie Jean" },
                    { 6, "Taylor Swift", 1, "An uptempo dance-pop anthem released in 2014, marking Swift's full transition to pop music. The lyrics express indifference toward detractors and negative media scrutiny, set to a catchy drum beat and saxophone line.", "https://www.youtube.com/watch?v=nfWlot6h_JM", "Shake It Off" },
                    { 7, "Harry Styles", 1, "A synth-pop and new wave track released in 2022. The song features a nostalgic 80s-inspired sound and explores themes of personal transition and changing relationships, becoming a record-breaking global hit.", "https://www.youtube.com/watch?v=H5v3kku4y6Q", "As It Was" },
                    { 8, "Dua Lipa", 1, "A high-energy track that combines disco and dance-pop influences. Featured on the album Future Nostalgia, it uses outer space metaphors to describe the feeling of falling in love and became a defining hit of the early 2020s.", "https://www.youtube.com/watch?v=TUVcZfQe-Kw", "Levitating" },
                    { 9, "Miles Davis", 2, "A 1959 masterpiece and the quintessential example of modal jazz. Using the Dorian mode, the piece relies on melodic improvisation rather than complex chord changes, setting a new direction for modern jazz music.", "https://www.youtube.com/watch?v=ylXk1LBvIqU", "So What" },
                    { 10, "Louis Armstrong", 2, "First recorded in 1967, this song is a hopeful anthem about the beauty of the world and the future of humanity. Armstrong's gravelly, soulful voice turned the track into a timeless classic recognized by all generations.", "https://www.youtube.com/watch?v=rBrd_3VMC3c", "What a Wonderful World" },
                    { 11, "Louis Armstrong", 2, "The title song from the 1964 musical of the same name. Armstrong's rendition was a massive success, famously displacing the Beatles from the top of the charts and earning him multiple Grammy Awards.", "https://www.youtube.com/watch?v=Kx2AYFvwxKY", "Hello, Dolly!" },
                    { 12, "Eminem", 3, "A lyrical powerhouse released in 2013, famous for its speed and technical complexity. With 1,560 words, it entered the Guinness World Records for the most words in a hit single, showcasing Eminem's immense rapping skill.", "https://www.youtube.com/watch?v=XbGs_qK2PQA", "Rap God" },
                    { 13, "Kendrick Lamar", 3, "A hard-hitting call to humility released in 2017. Produced by Mike WiLL Made-It, the song features a minimalist, piano-driven beat and sharp lyrics that critiques modern ego and social media culture.", "https://www.youtube.com/watch?v=tvTRZJ-4EyI", "Humble" },
                    { 14, "The Notorious B.I.G.", 3, "Widely considered one of the greatest hip-hop tracks ever made. Released in 1994, it tells Biggie's 'rags-to-riches' story, sampling Mtume's 'Juicy Fruit' to create a smooth, soulful, and triumphant anthem.", "https://www.youtube.com/watch?v=_JZom_gVfuw", "Juicy" },
                    { 15, "2Pac", 3, "Perhaps the most famous diss track in history, recorded during the peak of the East Coast-West Coast rivalry. Released in 1996, the song contains vicious insults aimed primarily at The Notorious B.I.G. and Bad Boy Records.", "https://www.youtube.com/watch?v=41qC3w3UUkU", "Hit 'Em Up" }
                });

            migrationBuilder.InsertData(
                table: "TriviaQuestions",
                columns: new[] { "Id", "Category", "CorrectAnswerIndex", "Options", "QuestionText" },
                values: new object[,]
                {
                    { 1, 0, 1, "[\"France\",\"Argentina\",\"Brazil\",\"Croatia\"]", "Who won the 2022 FIFA World Cup?" },
                    { 2, 0, 1, "[\"C. Ronaldo\",\"Lionel Messi\",\"Pele\",\"Ronaldinho\"]", "Which player has the most Ballon d'Or awards?" },
                    { 3, 0, 2, "[\"Italy\",\"Portugal\",\"Spain\",\"England\"]", "In which country is the Santiago Bernabéu stadium located?" },
                    { 4, 0, 1, "[\"30\",\"45\",\"60\",\"90\"]", "How many minutes does a standard half of football last?" },
                    { 5, 1, 1, "[\"Zelda\",\"Link\",\"Ganon\",\"Navi\"]", "Who is the main protagonist in 'The Legend of Zelda'?" },
                    { 6, 1, 2, "[\"Nintendo\",\"Microsoft\",\"Sony\",\"Sega\"]", "Which company launched the PlayStation console?" },
                    { 7, 1, 1, "[\"GTA V\",\"Minecraft\",\"Tetris\",\"Wii Sports\"]", "What is the best-selling video game of all time?" },
                    { 8, 1, 0, "[\"Portal\",\"Half-Life\",\"BioShock\",\"Skyrim\"]", "In which game does the phrase 'The cake is a lie' appear?" },
                    { 9, 2, 2, "[\"Helium\",\"Oxygen\",\"Hydrogen\",\"Nitrogen\"]", "What is the lightest chemical element?" },
                    { 10, 2, 1, "[\"Venus\",\"Mars\",\"Jupiter\",\"Saturn\"]", "Which planet is known as the 'Red Planet'?" },
                    { 11, 2, 2, "[\"Newton\",\"Tesla\",\"Einstein\",\"Hawking\"]", "Who developed the Theory of Relativity?" },
                    { 12, 2, 2, "[\"Iron\",\"Gold\",\"Mercury\",\"Lead\"]", "Which metal is liquid at room temperature?" },
                    { 13, 3, 1, "[\"1987\",\"1989\",\"1991\",\"1993\"]", "In which year did the Berlin Wall fall?" },
                    { 14, 3, 1, "[\"Buzz Aldrin\",\"Neil Armstrong\",\"Yuri Gagarin\",\"Michael Collins\"]", "Who was the first person to walk on the moon?" },
                    { 15, 3, 0, "[\"Marie Antoinette\",\"Catherine de\\u0027 Medici\",\"Anne of Austria\",\"Mary, Queen of Scots\"]", "Which French queen is famously associated with the phrase 'Let them eat cake'?" },
                    { 16, 3, 2, "[\"Romans\",\"Greeks\",\"Egyptians\",\"Mayans\"]", "Which ancient civilization built the Great Pyramid of Giza?" }
                });

            migrationBuilder.InsertData(
                table: "VideoGames",
                columns: new[] { "Id", "Category", "Description", "ImagePath", "Title" },
                values: new object[,]
                {
                    { 1, 0, "Counter-Strike 2 is a 2023 free-to-play first-person shooter developed by Valve. As the fifth main entry in the series and an update to CS:GO, it pits Counter-Terrorists against Terrorists in intense, objective-based tactical combat.", "/images/games/cs2.jpg", "Counter-Strike 2" },
                    { 2, 0, "A legendary military shooter franchise published by Activision. Known for its fast-paced action and cinematic storytelling, the series spans from historical battles to futuristic warfare, including the latest Black Ops 7.", "/images/games/call_of_duty.jpg", "Call of Duty" },
                    { 3, 0, "Developed by DICE and EA, Battlefield focuses on large-scale multiplayer warfare. It emphasizes teamwork, combined arms combat involving vehicles, and fully destructible environments across massive maps.", "/images/games/battlefield.jpg", "Battlefield" },
                    { 4, 1, "A masterpiece by CD Projekt Red, this open-world RPG follows Geralt of Rivia, a monster hunter for hire. Based on Sapkowski's novels, it offers deep storytelling and a rich fantasy world explored from a third-person perspective.", "/images/games/the_witcher_3.jpg", "The Witcher 3: Wild Hunt" },
                    { 5, 1, "Directed by Hidetaka Miyazaki with worldbuilding by George R. R. Martin, Elden Ring is a challenging action RPG. Players explore the Lands Between on a quest to repair the Elden Ring and become the new Elden Lord.", "/images/games/elden_ring.jpg", "Elden Ring" },
                    { 6, 1, "Larian Studios' cinematic RPG set in the Dungeons & Dragons universe. Players lead a party of characters infected with Illithid parasites, offering unparalleled player freedom and deep turn-based tactical combat.", "/images/games/baldurs_gate_3.jpg", "Baldur's Gate 3" },
                    { 7, 2, "A 2018 action-adventure game that reimagines Kratos' journey. Now set in the realm of Norse mythology, Kratos must navigate a dangerous world with his son Atreus, blending visceral combat with an emotional father-son story.", "/images/games/god_of_war.jpg", "God of War" },
                    { 8, 2, "Set in the Wild West in 1899, it follows Arthur Morgan, a member of the Van der Linde gang, as he struggles to survive in a changing world. The game features an open world where players can explore, fight, rob, and hunt.", "/images/games/red_dead_redemption_2.jpg", "Red Dead Redemption 2" },
                    { 9, 2, "Created by Naughty Dog, this post-apocalyptic series follows survivors Joel, Ellie, and Abby. It is renowned for its intense third-person combat, stealth mechanics, and a hauntingly beautiful, character-driven narrative.", "/images/games/the_last_of_us.jpg", "The Last of Us" },
                    { 10, 3, "Microsoft's longest-running software line and a pioneer in flight simulation. It allows players to pilot highly detailed aircraft across a realistic, 1:1 scale representation of the entire planet using satellite data.", "/images/games/microsoft_flight_simulator.jpg", "Microsoft Flight Simulator" },
                    { 11, 3, "A world-renowned life simulation series developed by Maxis. Players create 'Sims', build their homes, and manage their daily lives, careers, and relationships in one of the best-selling video game franchises ever.", "/images/games/the_sims.jpg", "The Sims" },
                    { 12, 3, "Developed by GIANTS Software, this series offers a realistic deep dive into modern agriculture. Players manage farms, breed livestock, and operate hundreds of authentic vehicles from real-world farming brands.", "/images/games/farming_simulator.jpg", "Farming Simulator" },
                    { 13, 3, "A highly popular truck simulator by SCS Software. Players travel across a vast map of Europe, delivering cargo in various trucks while managing their own logistics company and expanding their fleet.", "/images/games/euro_truck_simulator_2.jpg", "Euro Truck Simulator 2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunFacts");

            migrationBuilder.DropTable(
                name: "Jokes");

            migrationBuilder.DropTable(
                name: "Memes");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "TriviaQuestions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VideoGames");
        }
    }
}
