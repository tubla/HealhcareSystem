using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using notificationservice.function.Contracts;
using shared.V1.Events;
using System.Text.Json;

namespace notificationservice.function
{
    public class NotificationFunction(ILogger<NotificationFunction> _logger, IEmailClient _emailClient)
    {
        [Function("SendScheduledNotification")]
        public async Task RunScheduled(
             [EventHubTrigger(
                "appointment-scheduled",
                Connection = "EventHubConnection",
                ConsumerGroup = "$Default"
            )] EventData[] events)
        {
            foreach (var eventData in events)
            {
                try
                {
                    var appointmentEvent = JsonSerializer.Deserialize<AppointmentScheduledEvent>(eventData.Body.Span);
                    if (appointmentEvent == null)
                    {
                        _logger.LogError("SendScheduledNotification: Received null event object after deserialization");
                        continue;
                    }

                    _logger.LogInformation(
                        $"Processing scheduled appointment {appointmentEvent.AppointmentId} for patient {appointmentEvent.PatientId}"
                    );

                    // Send email notification
                    await _emailClient.SendEmailAsync(
                        await GetPatientEmailAsync(appointmentEvent.PatientId),
                        "Appointment Scheduled",
                        $"Your appointment with ID {appointmentEvent.AppointmentId} is scheduled for {appointmentEvent.AppointmentDateTime:MM/dd/yyyy HH:mm} with Doctor ID {appointmentEvent.DoctorId}."
                    );

                    _logger.LogInformation(
                        $"Email sent to patient for scheduled appointment at {appointmentEvent.AppointmentDateTime}"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing scheduled event for Appointment : {ex.Message}");
                }
            }
        }

        [Function("SendCancelledNotification")]
        public async Task RunCancelled(
            [EventHubTrigger(
                "appointment-cancelled",
                Connection = "EventHubConnection",
                ConsumerGroup = "$Default"
            )] EventData[] events)
        {
            foreach (var eventData in events)
            {
                try
                {
                    var appointmentEvent = JsonSerializer.Deserialize<AppointmentCancelledEvent>(eventData.Body.Span);
                    if (appointmentEvent == null)
                    {
                        _logger.LogError("SendCancelledNotification: Received null event object after deserialization");
                        continue;
                    }

                    _logger.LogInformation(
                        $"Processing cancelled appointment {appointmentEvent.AppointmentId} for patient {appointmentEvent.PatientId}"
                    );

                    // Send email notification
                    await _emailClient.SendEmailAsync(
                        await GetPatientEmailAsync(appointmentEvent.PatientId),
                        "Appointment Cancelled",
                        $"Your appointment with ID {appointmentEvent.AppointmentId} scheduled for {appointmentEvent.AppointmentDateTime:MM/dd/yyyy HH:mm} with Doctor ID {appointmentEvent.DoctorId} has been cancelled."
                    );

                    _logger.LogInformation(
                        $"Email sent to patient for cancelled appointment at {appointmentEvent.AppointmentDateTime}"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing cancelled event for Appointment: {ex.Message}");
                }
            }
        }

        private async Task<string> GetPatientEmailAsync(int patientId)
        {
            // Simulate patient email retrieval (replace with actual Patient Service API call)
            // Example: var patient = await _patientService.GetPatientAsync(patientId);
            // return patient.Email;
            return $"patient{patientId}@example.com";
        }
    }
}
