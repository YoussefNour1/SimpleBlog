namespace SimpleBlog.Services
{
    public class ConsoleMessage : IMessageSender
    {
        public void SendMessage(string recipient, string subject, string message)
        {
            Console.WriteLine($"To: {recipient}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
