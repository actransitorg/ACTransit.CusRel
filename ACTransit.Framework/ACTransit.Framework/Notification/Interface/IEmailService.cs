namespace ACTransit.Framework.Notification.Interface
{
    public interface IEmailService
    {
        string ServerAddress { get; }

        void Send(EmailPayload payload);
    }
}