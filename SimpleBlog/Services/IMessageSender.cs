namespace SimpleBlog.Services
{
    public interface IMessageSender
    {
        public void SendMessage(string recipient, string subject, string message);
    }
}
