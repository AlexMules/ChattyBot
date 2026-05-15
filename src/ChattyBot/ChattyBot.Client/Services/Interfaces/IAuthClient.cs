using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Client.Services.Interfaces
{
    public interface IAuthClient
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
    }
}
