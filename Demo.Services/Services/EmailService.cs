using Demo.Entities.ViewModels;
using MimeKit;

namespace Demo.Services.Services
{
    public class EmailService : IEmailService
    {

        private readonly SendEmailModel _emailConfig;

        public EmailService(SendEmailModel emailConfig)
        {
            _emailConfig = emailConfig;
        }

        private static Dictionary<string, (string token, DateTime expiration)> _resetTokens = new();
        public async Task<bool> SendEmailToResetPasswordAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Demo", _emailConfig.From));
                message.To.Add(new MailboxAddress("DemoMember", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, (int)_emailConfig.Port, false);
                    await client.AuthenticateAsync(_emailConfig.From, _emailConfig.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
      

    }
}
