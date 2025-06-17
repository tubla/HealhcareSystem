using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using notificationservice.function.Contracts;

namespace notificationservice.function;

public class AzureCommunicationEmailClient : IEmailClient
{
    private readonly EmailClient _emailClient;
    private readonly string _fromEmail;

    public AzureCommunicationEmailClient(IConfiguration configuration)
    {
        var connectionString = configuration["AzureCommunication:ConnectionString"]
            ?? throw new ArgumentNullException(nameof(configuration), "AzureCommunication:ConnectionString is not configured.");

        _fromEmail = configuration["AzureCommunication:FromEmail"]
            ?? throw new ArgumentNullException(nameof(configuration), "AzureCommunication:FromEmail is not configured.");

        _emailClient = new EmailClient(connectionString);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email cannot be empty.", nameof(toEmail));

        var message = new EmailMessage(
            senderAddress: _fromEmail,
            content: new EmailContent(subject)
            {
                PlainText = body,
                Html = $"<p>{body}</p>"
            },
            recipients: new EmailRecipients(new List<EmailAddress>
            {
                new(toEmail)
            })
        );

        // Send the email and wait for completion
        EmailSendOperation operation = await _emailClient.SendAsync(
            wait: WaitUntil.Completed,
            message: message
        );

        if (operation.HasCompleted && operation.Value.Status != EmailSendStatus.Succeeded)
        {
            throw new InvalidOperationException($"Email send failed. Status: {operation.Value.Status}");
        }
    }
}
