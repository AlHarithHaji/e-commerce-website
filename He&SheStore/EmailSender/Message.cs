namespace He_SheStore.EmailSender
{
    public class Message
    {
        public string Messageto { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string calback { get; set; }



        public Message(string messageto, string subject, string content, string calback)
        {
            this.Messageto = messageto;
            Subject = subject;
            Content = content;
            this.calback = calback;
        }
    }
}
