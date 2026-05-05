using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Interfaces
{
    public interface IAccountManagerService
    {
        Task<AuthResponseDTO> ChangeUsernameAsync(int userId, ChangeUsernameDTO dto);
        Task<AuthResponseDTO> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
        Task<AuthResponseDTO> ChangeEmailAsync(int userId, ChangeEmailDTO dto);
    }
}
