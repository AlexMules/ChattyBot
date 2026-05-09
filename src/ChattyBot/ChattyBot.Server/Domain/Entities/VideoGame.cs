using ChattyBot.Server.Domain.Enums;

namespace ChattyBot.Server.Domain.Entities
{
    public class VideoGame
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public GameCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}
