namespace ChattyBot.Server.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = "avatar1.png";

        // Concurrency token - used for optimistic concurrency control
        public DateTime Version { get; set; }
    }
}
