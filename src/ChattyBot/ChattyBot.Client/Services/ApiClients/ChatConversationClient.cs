using ChattyBot.Shared.Contracts.DTO;
using System.Net.Http.Json;

namespace ChattyBot.Client.Services.ApiClients
{
    public class ChatConversationClient
    {
        private readonly HttpClient _httpClient;

        public ChatConversationClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ChatConversationDTO>> GetConversationsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ChatConversationDTO>>("api/ChatConversation/conversations")
                   ?? new List<ChatConversationDTO>();
        }

        public async Task<ChatConversationDTO?> CreateConversationAsync(CreateChatDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ChatConversation", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChatConversationDTO>();
            }

            return null;
        }

        public async Task<bool> DeleteConversationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ChatConversation/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RenameConversationAsync(int id, RenameChatDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ChatConversation/{id}/rename", dto);

            return response.IsSuccessStatusCode;
        }
    }
}