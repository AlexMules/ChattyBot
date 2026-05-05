using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Infrastructure.Security.Interfaces;

namespace ChattyBot.Server.Application.Services
{
    public class AccountManagerService : IAccountManagerService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public AccountManagerService(IUserRepository userRepository, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponseDTO> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDTO { IsSuccess = false, ErrorMessage = "User not found!" };
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return new AuthResponseDTO { IsSuccess = false, ErrorMessage = "The current password is wrong!" };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.SaveChangesAsync();

            return new AuthResponseDTO { IsSuccess = true };
        }

        public async Task<AuthResponseDTO> ChangeUsernameAsync(int userId, ChangeUsernameDTO dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDTO { IsSuccess = false, ErrorMessage = "User not found!" };
            }

            string oldUsername = user.Username;
            user.Username = dto.NewUsername;

            try
            {
                var newToken = _tokenGenerator.GenerateJwtToken(user);
                await _userRepository.SaveChangesAsync();

                return new AuthResponseDTO { IsSuccess = true, Token = newToken };
            }
            catch (Exception)
            {
                user.Username = oldUsername;

                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    ErrorMessage = "An internal error occurred! No changes were made!"
                };
            }
        }

        public async Task<AuthResponseDTO> ChangeEmailAsync(int userId, ChangeEmailDTO dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDTO { IsSuccess = false, ErrorMessage = "User not found!" };
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Incorrect password!" };
            }

            var emailExists = await _userRepository.GetByEmailAsync(dto.NewEmail);
            if (emailExists != null)
            {
                return new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Email address is already used!" };
            }

            string oldEmail = user.Email;
            user.Email = dto.NewEmail;

            try
            {
                var newToken = _tokenGenerator.GenerateJwtToken(user);
                await _userRepository.SaveChangesAsync();

                return new AuthResponseDTO { IsSuccess = true, Token = newToken };
            }
            catch (Exception)
            {
                user.Email = oldEmail;

                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    ErrorMessage = "An internal error occurred! No changes were made!"
                };
            }
        }
    }
}
