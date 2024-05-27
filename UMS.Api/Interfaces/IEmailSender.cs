using UMS.Api.Services;

namespace UMS.Api.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message, string? email, string callback);
    }
}
