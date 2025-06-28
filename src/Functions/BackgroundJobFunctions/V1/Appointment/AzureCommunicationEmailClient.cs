using Azure;
using Azure.Communication.Email;
using Azure.Identity;
using BackgroundJobFunctions.V1.Contracts;
using Microsoft.Extensions.Configuration;

namespace BackgroundJobFunctions.V1.Appointment;

public class AzureCommunicationEmailClient : IEmailClient
{
    private readonly EmailClient _emailClient;
    private const string _fromEmail =
        "donotreply@b4e1c6cf-d68c-461e-a3fe-10f79f2c41fb.azurecomm.net";

    public AzureCommunicationEmailClient(IConfiguration _configuration)
    {
        var endPoint =
            _configuration["CommunicationServiceEndpoint"]
            ?? throw new ArgumentNullException(
                nameof(_configuration),
                "CommunicationServiceEndpoint url is not available."
            );
        _emailClient = new EmailClient(new Uri(endPoint), new DefaultAzureCredential());
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email cannot be empty.", nameof(toEmail));

        var message = new EmailMessage(
            senderAddress: _fromEmail,
            content: new EmailContent(subject) { PlainText = body, Html = $"<p>{body}</p>" },
            recipients: new EmailRecipients(new List<EmailAddress> { new(toEmail) })
        );

        // Send the email and wait for completion
        EmailSendOperation operation = await _emailClient.SendAsync(
            wait: WaitUntil.Completed,
            message: message
        );

        if (operation.HasCompleted && operation.Value.Status != EmailSendStatus.Succeeded)
        {
            throw new InvalidOperationException(
                $"Email send failed. Status: {operation.Value.Status}"
            );
        }
    }
}
