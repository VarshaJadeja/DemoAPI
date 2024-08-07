namespace Demo.Services.Services;

public interface IEmailService
{
    public Task<bool> SendEmailToResetPasswordAsync(string toEmail, string subject, string body);
}