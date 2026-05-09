using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class VideoGameConfiguration : IEntityTypeConfiguration<VideoGame>
    {
        public void Configure(EntityTypeBuilder<VideoGame> builder)
        {
            builder.HasData(
                // FPS Games
                new VideoGame
                {
                    Id = 1,
                    Title = "Counter-Strike 2",
                    Category = GameCategory.FPS,
                    Description = "Counter-Strike 2 is a 2023 free-to-play first-person shooter developed by Valve. As the fifth main entry in the series and an update to CS:GO, it pits Counter-Terrorists against Terrorists in intense, objective-based tactical combat.",
                    ImagePath = "/images/games/cs2.jpg"
                },
                new VideoGame
                {
                    Id = 2,
                    Title = "Call of Duty",
                    Category = GameCategory.FPS,
                    Description = "A legendary military shooter franchise published by Activision. Known for its fast-paced action and cinematic storytelling, the series spans from historical battles to futuristic warfare, including the latest Black Ops 7.",
                    ImagePath = "/images/games/call_of_duty.jpg"
                },
                new VideoGame
                {
                    Id = 3,
                    Title = "Battlefield",
                    Category = GameCategory.FPS,
                    Description = "Developed by DICE and EA, Battlefield focuses on large-scale multiplayer warfare. It emphasizes teamwork, combined arms combat involving vehicles, and fully destructible environments across massive maps.",
                    ImagePath = "/images/games/battlefield.jpg"
                },

                // RPG Games
                new VideoGame
                {
                    Id = 4,
                    Title = "The Witcher 3: Wild Hunt",
                    Category = GameCategory.RPG,
                    Description = "A masterpiece by CD Projekt Red, this open-world RPG follows Geralt of Rivia, a monster hunter for hire. Based on Sapkowski's novels, it offers deep storytelling and a rich fantasy world explored from a third-person perspective.",
                    ImagePath = "/images/games/the_witcher_3.jpg"
                },
                new VideoGame
                {
                    Id = 5,
                    Title = "Elden Ring",
                    Category = GameCategory.RPG,
                    Description = "Directed by Hidetaka Miyazaki with worldbuilding by George R. R. Martin, Elden Ring is a challenging action RPG. Players explore the Lands Between on a quest to repair the Elden Ring and become the new Elden Lord.",
                    ImagePath = "/images/games/elden_ring.jpg"
                },
                new VideoGame
                {
                    Id = 6,
                    Title = "Baldur's Gate 3",
                    Category = GameCategory.RPG,
                    Description = "Larian Studios' cinematic RPG set in the Dungeons & Dragons universe. Players lead a party of characters infected with Illithid parasites, offering unparalleled player freedom and deep turn-based tactical combat.",
                    ImagePath = "/images/games/baldurs_gate_3.jpg"
                },

                // Action-Adventure Games
                new VideoGame
                {
                    Id = 7,
                    Title = "God of War",
                    Category = GameCategory.ActionAdventure,
                    Description = "A 2018 action-adventure game that reimagines Kratos' journey. Now set in the realm of Norse mythology, Kratos must navigate a dangerous world with his son Atreus, blending visceral combat with an emotional father-son story.",
                    ImagePath = "/images/games/god_of_war.jpg"
                },
                new VideoGame
                {
                    Id = 8,
                    Title = "Red Dead Redemption 2",
                    Category = GameCategory.ActionAdventure,
                    Description = "Set in the Wild West in 1899, it follows Arthur Morgan, a member of the Van der Linde gang, as he struggles to survive in a changing world. The game features an open world where players can explore, fight, rob, and hunt.",
                    ImagePath = "/images/games/red_dead_redemption_2.jpg"
                },
                new VideoGame
                {
                    Id = 9,
                    Title = "The Last of Us",
                    Category = GameCategory.ActionAdventure,
                    Description = "Created by Naughty Dog, this post-apocalyptic series follows survivors Joel, Ellie, and Abby. It is renowned for its intense third-person combat, stealth mechanics, and a hauntingly beautiful, character-driven narrative.",
                    ImagePath = "/images/games/the_last_of_us.jpg"
                },

                // Simulation Games
                new VideoGame
                {
                    Id = 10,
                    Title = "Microsoft Flight Simulator",
                    Category = GameCategory.Simulation,
                    Description = "Microsoft's longest-running software line and a pioneer in flight simulation. It allows players to pilot highly detailed aircraft across a realistic, 1:1 scale representation of the entire planet using satellite data.",
                    ImagePath = "/images/games/microsoft_flight_simulator.jpg"
                },
                new VideoGame
                {
                    Id = 11,
                    Title = "The Sims",
                    Category = GameCategory.Simulation,
                    Description = "A world-renowned life simulation series developed by Maxis. Players create 'Sims', build their homes, and manage their daily lives, careers, and relationships in one of the best-selling video game franchises ever.",
                    ImagePath = "/images/games/the_sims.jpg"
                },
                new VideoGame
                {
                    Id = 12,
                    Title = "Farming Simulator",
                    Category = GameCategory.Simulation,
                    Description = "Developed by GIANTS Software, this series offers a realistic deep dive into modern agriculture. Players manage farms, breed livestock, and operate hundreds of authentic vehicles from real-world farming brands.",
                    ImagePath = "/images/games/farming_simulator.jpg"
                },
                new VideoGame
                {
                    Id = 13,
                    Title = "Euro Truck Simulator 2",
                    Category = GameCategory.Simulation,
                    Description = "A highly popular truck simulator by SCS Software. Players travel across a vast map of Europe, delivering cargo in various trucks while managing their own logistics company and expanding their fleet.",
                    ImagePath = "/images/games/euro_truck_simulator_2.jpg"
                }
            );
        }
    }
}