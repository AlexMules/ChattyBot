using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Services
{
    public class TriviaService : ITriviaService
    {
        private readonly ITriviaRepository _repository;

        public TriviaService(ITriviaRepository repository) => _repository = repository;

        public async Task<TriviaQuestionDTO?> GetQuestionAsync(TriviaCategory? category = null)
        {
            var question = category == null
                ? await _repository.GetRandomAsync()
                : await _repository.GetRandomByCategoryAsync(category.Value);

            if (question == null)
            {
                return null;
            }

            return new TriviaQuestionDTO(question.Id, question.QuestionText, question.Options);
        }

        public async Task<TriviaCheckResponseDTO> VerifyAnswerAsync(TriviaCheckRequestDTO request)
        {
            var question = await _repository.GetByIdAsync(request.QuestionId);

            if (question == null)
            {
                throw new KeyNotFoundException("Question was not found!");
            }

            return new TriviaCheckResponseDTO(
                question.CorrectAnswerIndex == request.AnswerIndex,
                question.CorrectAnswerIndex
            );
        }
    }
}
