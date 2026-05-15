using System.ComponentModel.DataAnnotations;

namespace ChattyBot.Shared.Contracts.DTO
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Current password is required!")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required!")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "New password must contain at least one uppercase letter, one digit, and one special character!")]
        public string NewPassword { get; set; } = string.Empty;

        public ChangePasswordDTO(string currentPassword, string newPassword)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }

        public ChangePasswordDTO() { }
    }
}
