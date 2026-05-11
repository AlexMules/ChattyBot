using ChattyBot.Server.Domain.Enums;
using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Interfaces
{
    public interface ITriviaService
    {
        Task<TriviaQuestionDTO?> GetQuestionAsync(TriviaCategory? category = null);
        Task<TriviaCheckResponseDTO> VerifyAnswerAsync(TriviaCheckRequestDTO request, int messageId);
    }
}
