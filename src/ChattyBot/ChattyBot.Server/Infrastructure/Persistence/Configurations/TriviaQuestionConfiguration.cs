using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace ChattyBot.Server.Infrastructure.Persistence.Configurations
{
    public class TriviaQuestionConfiguration : IEntityTypeConfiguration<TriviaQuestion>
    {
        public void Configure(EntityTypeBuilder<TriviaQuestion> builder)
        {
            var optionsSerializerOptions = (JsonSerializerOptions?)null;

            builder.Property(e => e.Options)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, optionsSerializerOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, optionsSerializerOptions) ?? new List<string>(),
                    new ValueComparer<List<string>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            builder.HasData(
                // football questions
                new TriviaQuestion { Id = 1, Category = TriviaCategory.Football, QuestionText = "Who won the 2022 FIFA World Cup?", Options = new List<string> { "France", "Argentina", "Brazil", "Croatia" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 2, Category = TriviaCategory.Football, QuestionText = "Which player has the most Ballon d'Or awards?", Options = new List<string> { "C. Ronaldo", "Lionel Messi", "Pele", "Ronaldinho" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 3, Category = TriviaCategory.Football, QuestionText = "In which country is the Santiago Bernabéu stadium located?", Options = new List<string> { "Italy", "Portugal", "Spain", "England" }, CorrectAnswerIndex = 2 },
                new TriviaQuestion { Id = 4, Category = TriviaCategory.Football, QuestionText = "How many minutes does a standard half of football last?", Options = new List<string> { "30", "45", "60", "90" }, CorrectAnswerIndex = 1 },

                // gaming questions
                new TriviaQuestion { Id = 5, Category = TriviaCategory.Gaming, QuestionText = "Who is the main protagonist in 'The Legend of Zelda'?", Options = new List<string> { "Zelda", "Link", "Ganon", "Navi" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 6, Category = TriviaCategory.Gaming, QuestionText = "Which company launched the PlayStation console?", Options = new List<string> { "Nintendo", "Microsoft", "Sony", "Sega" }, CorrectAnswerIndex = 2 },
                new TriviaQuestion { Id = 7, Category = TriviaCategory.Gaming, QuestionText = "What is the best-selling video game of all time?", Options = new List<string> { "GTA V", "Minecraft", "Tetris", "Wii Sports" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 8, Category = TriviaCategory.Gaming, QuestionText = "In which game does the phrase 'The cake is a lie' appear?", Options = new List<string> { "Portal", "Half-Life", "BioShock", "Skyrim" }, CorrectAnswerIndex = 0 },

                // science questions
                new TriviaQuestion { Id = 9, Category = TriviaCategory.Science, QuestionText = "What is the lightest chemical element?", Options = new List<string> { "Helium", "Oxygen", "Hydrogen", "Nitrogen" }, CorrectAnswerIndex = 2 },
                new TriviaQuestion { Id = 10, Category = TriviaCategory.Science, QuestionText = "Which planet is known as the 'Red Planet'?", Options = new List<string> { "Venus", "Mars", "Jupiter", "Saturn" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 11, Category = TriviaCategory.Science, QuestionText = "Who developed the Theory of Relativity?", Options = new List<string> { "Newton", "Tesla", "Einstein", "Hawking" }, CorrectAnswerIndex = 2 },
                new TriviaQuestion { Id = 12, Category = TriviaCategory.Science, QuestionText = "Which metal is liquid at room temperature?", Options = new List<string> { "Iron", "Gold", "Mercury", "Lead" }, CorrectAnswerIndex = 2 },

                // history questions
                new TriviaQuestion { Id = 13, Category = TriviaCategory.History, QuestionText = "In which year did the Berlin Wall fall?", Options = new List<string> { "1987", "1989", "1991", "1993" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 14, Category = TriviaCategory.History, QuestionText = "Who was the first person to walk on the moon?", Options = new List<string> { "Buzz Aldrin", "Neil Armstrong", "Yuri Gagarin", "Michael Collins" }, CorrectAnswerIndex = 1 },
                new TriviaQuestion { Id = 15, Category = TriviaCategory.History, QuestionText = "Which French queen is famously associated with the phrase 'Let them eat cake'?", Options = new List<string> { "Marie Antoinette", "Catherine de' Medici", "Anne of Austria", "Mary, Queen of Scots" }, CorrectAnswerIndex = 0 },
                new TriviaQuestion { Id = 16, Category = TriviaCategory.History, QuestionText = "Which ancient civilization built the Great Pyramid of Giza?", Options = new List<string> { "Romans", "Greeks", "Egyptians", "Mayans" }, CorrectAnswerIndex = 2 }
            );
        }
    }
}