using ChattyBot.Shared.Contracts.DTO;
using System.Net.Http.Json;

namespace ChattyBot.Client.Services.ApiClients
{
    public class ChatMessageClient
    {
        private readonly HttpClient _httpClient;

        public ChatMessageClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ChatMessageDTO>> GetChatHistoryAsync(int chatId)
        {
            return await _httpClient.GetFromJsonAsync<List<ChatMessageDTO>>($"api/ChatMessage/conversation/{chatId}")
                   ?? new List<ChatMessageDTO>();
        }

        public async Task<List<ChatMessageDTO>> SendMessageAsync(int chatId, SendMessageDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/ChatMessage/{chatId}/send", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ChatMessageDTO>>()
                       ?? new List<ChatMessageDTO>();
            }

            return new List<ChatMessageDTO>();
        }
    }
}