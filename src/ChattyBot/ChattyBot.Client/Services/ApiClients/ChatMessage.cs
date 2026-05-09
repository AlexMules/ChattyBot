namespace ChattyBot.Client.Services.ApiClients
{
    //mockup class, MUST DELETE after the backend is implemented
    public class ChatMessage
    {
        public string Content { get; set; } = "";
        public MessageSender Sender { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
