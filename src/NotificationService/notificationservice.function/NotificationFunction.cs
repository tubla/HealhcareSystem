using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using shared.Events;

namespace notificationservice.function
{
    public class NotificationFunction(ILogger<NotificationFunction> _logger)
    {
        [Function("SendNotification")]
        public async Task Run(
            [EventHubTrigger(
                "appointment-scheduled",
                Connection = "EventHubConnection",
                ConsumerGroup = "$Default" // Default consumer group configured in azure event hub
            )]
                string[] events
        )
        {
            foreach (var eventData in events)
            {
                try
                {
                    var appointmentEvent = JsonSerializer.Deserialize<AppointmentScheduledEvent>(
                        eventData
                    );
                    if (appointmentEvent == null)
                    {
                        _logger.LogError(
                            "NotificationFunction : Received null event object after deserialization"
                        );
                        continue;
                    }
                    _logger.LogInformation(
                        $"Processing appointment {appointmentEvent.AppointmentId} for patient {appointmentEvent.PatientId}"
                    );

                    // Simulate sending email (replace with SendGrid or Azure Communication Services)
                    _logger.LogInformation(
                        $"Email sent to patient for appointment at {appointmentEvent.AppointmentDateTime}"
                    );
                    // Example: await _emailClient.SendEmailAsync("patient@example.com", "Appointment Scheduled", $"Your appointment is at {appointmentEvent.AppointmentDateTime}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing event: {ex.Message}");
                }
            }
        }
    }
}
