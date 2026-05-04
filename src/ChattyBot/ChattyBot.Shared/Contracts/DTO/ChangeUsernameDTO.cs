using System.ComponentModel.DataAnnotations;

namespace ChattyBot.Shared.Contracts.DTO
{
    public class ChangeUsernameDTO
    {
        [Required(ErrorMessage = "New username is required!")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers and underscores!")]
        public string NewUsername { get; set; } = string.Empty;
    }
}
