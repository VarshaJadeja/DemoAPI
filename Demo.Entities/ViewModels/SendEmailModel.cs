namespace Demo.Entities.ViewModels;

public class SendEmailModel
{
    public string? Email { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? SmtpServer { get; set; }
    public long Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
