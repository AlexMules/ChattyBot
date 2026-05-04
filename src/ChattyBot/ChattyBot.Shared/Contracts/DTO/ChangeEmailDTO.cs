using System.ComponentModel.DataAnnotations;

namespace ChattyBot.Shared.Contracts.DTO
{
    public class ChangeEmailDTO
    {
        [Required(ErrorMessage = "New email address is required!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter a valid email address! (ex: user@domain.com)")]
        public string NewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current password is required!")]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
