namespace ChattyBot.Server.Domain.Entities
{
    public class FunFact
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;
    }
}
