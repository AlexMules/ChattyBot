using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using System.Text.Json;

namespace ChattyBot.Server.Application.Services
{
    public class TriviaService : ITriviaService
    {
        private readonly ITriviaRepository _triviaRepository;
        private readonly IChatMessageRepository _messageRepository;

        public TriviaService(ITriviaRepository triviaRepository, IChatMessageRepository messageRepository)
        {
            _triviaRepository = triviaRepository;
            _messageRepository = messageRepository;
        }

        public async Task<TriviaQuestionDTO?> GetQuestionAsync(TriviaCategory? category = null)
        {
            var question = category == null
                ? await _triviaRepository.GetRandomAsync()
                : await _triviaRepository.GetRandomByCategoryAsync(category.Value);

            if (question == null)
            {
                return null;
            }

            return new TriviaQuestionDTO(question.Id, question.QuestionText, question.Options);
        }

        public async Task<TriviaCheckResponseDTO> VerifyAnswerAsync(TriviaCheckRequestDTO request, int messageId)
        {
            var question = await _triviaRepository.GetByIdAsync(request.QuestionId);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found!");
            }

            bool isCorrect = question.CorrectAnswerIndex == request.AnswerIndex;

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message != null)
            {
                var updatedDto = new TriviaQuestionDTO(
                    question.Id,
                    question.QuestionText,
                    question.Options,
                    request.AnswerIndex,
                    question.CorrectAnswerIndex
                );

                message.Content = JsonSerializer.Serialize(updatedDto);

                await _messageRepository.UpdateAsync(message);
            }

            return new TriviaCheckResponseDTO(isCorrect, question.CorrectAnswerIndex);
        }
    }
}
