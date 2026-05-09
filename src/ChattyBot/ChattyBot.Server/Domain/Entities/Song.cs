using ChattyBot.Server.Domain.Enums;

namespace ChattyBot.Server.Domain.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public MusicCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SongPath { get; set; } = string.Empty;
    }
}
