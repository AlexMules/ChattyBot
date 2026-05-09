using ChattyBot.Server.Domain.Enums;

namespace ChattyBot.Server.Domain.Entities
{
    public class TriviaQuestion
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectAnswerIndex { get; set; }
        public TriviaCategory Category { get; set; }
    }
}
