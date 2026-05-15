using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Client.Services.Interfaces
{
    public interface IManageAccountClient
    {
        Task<AuthResponseDTO> ChangePasswordAsync(ChangePasswordDTO dto);
        Task<AuthResponseDTO> ChangeUsernameAsync(ChangeUsernameDTO dto);
        Task<AuthResponseDTO> ChangeEmailAsync(ChangeEmailDTO dto);
    }
}
