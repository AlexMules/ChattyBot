using System.ComponentModel.DataAnnotations;

namespace ChattyBot.Shared.Contracts.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email field is mandatory!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter a valid email address! (ex: user@domain.com)")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password field is mandatory!")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one digit, and one special character!")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username field is mandatory!")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers and underscores!")]
        public string Username { get; set; } = string.Empty;

        public string AvatarPath { get; set; } = "avatar1.png";

        public RegisterDTO(string email, string username, string password, string avatarPath = "avatar1.png")
        {
            Email = email;
            Username = username;
            Password = password;
            AvatarPath = avatarPath;
        }

        public RegisterDTO() { }
    }
}