using ChattyBot.Shared.Contracts.DTO;
using System.Net.Http.Json;

namespace ChattyBot.Client.Services.ApiClients
{
    public class AuthClient
    {
        private readonly HttpClient _httpClient;

        public AuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>()
                   ?? new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Server error" };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>()
                   ?? new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Server error" };
        }
    }
}