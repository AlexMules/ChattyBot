using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using System.Net.Http.Json;

namespace ChattyBot.Client.Services.ApiClients
{
    public class TriviaClient : ITriviaClient
    {
        private readonly HttpClient _httpClient;

        public TriviaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TriviaCheckResponseDTO?> VerifyAnswerAsync(TriviaCheckRequestDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/trivia/verify", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TriviaCheckResponseDTO>();
            }

            return null;
        }
    }
}
