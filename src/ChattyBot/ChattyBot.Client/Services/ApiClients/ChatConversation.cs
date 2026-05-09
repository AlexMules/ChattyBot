namespace ChattyBot.Client.Services.ApiClients
{
    //mockup class, MUST DELETE after the backend is implemented
    public class ChatConversation
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ChatMessage> Messages { get; set; } = new();
    }
}
