namespace ChattyBot.Shared.Contracts.DTO
{
    public record TriviaQuestionDTO(
        int QuestionId,
        string QuestionText,
        List<string> Options,
        int? UserAnswerIndex = null,
        int? CorrectAnswerIndex = null
    );
}
