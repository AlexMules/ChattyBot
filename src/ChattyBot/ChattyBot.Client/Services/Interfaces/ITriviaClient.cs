using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Client.Services.Interfaces
{
    public interface ITriviaClient
    {
        Task<TriviaCheckResponseDTO?> VerifyAnswerAsync(TriviaCheckRequestDTO dto);
    }
}
