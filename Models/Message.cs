namespace regmock.Models
{
    public class Message
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Sender { get; set; }
        public DateTime? Date { get; set; }
        public MessageType? MessageType { get; set; }
    }
}
