namespace notificationservice.function.Contracts;

public interface IEmailClient
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
