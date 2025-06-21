using System.Threading.Tasks;

namespace BackgroundJobFunctions.V1.Contracts;

public interface IEmailClient
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
