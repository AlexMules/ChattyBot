using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using System.Net.Http.Json;

namespace ChattyBot.Client.Services.ApiClients
{
    public class ManageAccountClient : IManageAccountClient
    {
        private readonly HttpClient _httpClient;

        public ManageAccountClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponseDTO> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/change-password", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>()
                   ?? new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Server error" };
        }

        public async Task<AuthResponseDTO> ChangeUsernameAsync(ChangeUsernameDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/change-username", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>()
                   ?? new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Server error" };
        }

        public async Task<AuthResponseDTO> ChangeEmailAsync(ChangeEmailDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/change-email", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>()
                   ?? new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Server error" };
        }
    }
}