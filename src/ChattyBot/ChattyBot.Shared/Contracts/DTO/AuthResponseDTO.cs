namespace ChattyBot.Shared.Contracts.DTO
{
    public class AuthResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
