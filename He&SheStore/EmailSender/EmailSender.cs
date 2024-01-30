using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace He_SheStore.EmailSender
{
    public class EmailSender : IMailSender
    {
        public readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuratin;
        public EmailSender(ISendGridClient sendGridClient, IConfiguration configuratin)
        {
            _sendGridClient = sendGridClient;
            _configuratin = configuratin;
        }

        public void MessageSend(Message message)
        {
            try
            {
                string fromEmail = _configuratin.GetSection("SendGridEmailSetting").GetValue<string>("FromEmail");
                string FromName = _configuratin.GetSection("SendGridEmailSetting").GetValue<string>("FromName");
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(fromEmail, FromName),
                    Subject = message.Subject,
                    HtmlContent = message.Content,
                };
                msg.AddTo(message.Messageto);
                _sendGridClient.SendEmailAsync(msg);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
