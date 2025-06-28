using System.Text.Json;
using Azure.Messaging.EventHubs;
using BackgroundJobFunctions.V1.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using shared.V1.Events;

namespace BackgroundJobFunctions.V1.Appointment
{
    public class ScheduledNotificationFunction(
        IEmailClient _emailClient,
        IConfiguration _configuration,
        ILogger<ScheduledNotificationFunction> _logger
    )
    {
        [FunctionName("ScheduledNotificationFunction")]
        public async Task RunScheduled(
            [EventHubTrigger(
                EventNames.AppointmentScheduled,
                Connection = "EventHubConnection", // from configuration -> check Program.cs
                ConsumerGroup = "$Default"
            )]
                EventData[] events
        )
        {
            foreach (var eventData in events)
            {
                try
                {
                    var appointmentEvent = JsonSerializer.Deserialize<AppointmentScheduledEvent>(
                        eventData.Body.Span
                    );
                    if (appointmentEvent == null)
                    {
                        _logger.LogError(
                            "SendScheduledNotification: Received null event object after deserialization"
                        );
                        continue;
                    }

                    _logger.LogInformation(
                        $"Processing scheduled appointment {appointmentEvent.AppointmentId} for patient {appointmentEvent.PatientId}"
                    );

                    var sqlConnectionString = _configuration["SqlConnection"]; // from configuration -> check Program.cs
                    using var connection = new SqlConnection(sqlConnectionString);
                    await connection.OpenAsync();
                    var query = $"SELECT email FROM [healthcare].[patient] WHERE patient_id=@id)";
                    string email = await GetDataFromDb(
                        query,
                        appointmentEvent.PatientId,
                        connection
                    );

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        _logger.LogWarning(
                            $"No email found for patient ID {appointmentEvent.PatientId}"
                        );
                        continue;
                    }

                    query = $"SELECT name FROM [healthcare].[doctor] WHERE doctor_id=@id)";
                    string doctorName = await GetDataFromDb(
                        query,
                        appointmentEvent.DoctorId,
                        connection
                    );

                    // Send email notification
                    string doctorInfo = !string.IsNullOrWhiteSpace(doctorName)
                        ? $"with Dr. {doctorName}."
                        : string.Empty;
                    await _emailClient.SendEmailAsync(
                        email,
                        "Appointment Scheduled",
                        $"Your appointment with ID {appointmentEvent.AppointmentId} is scheduled for {appointmentEvent.AppointmentDateTime:MM/dd/yyyy HH:mm} {doctorInfo}"
                    );

                    _logger.LogInformation(
                        $"Email sent to patient for scheduled appointment at {appointmentEvent.AppointmentDateTime}"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        $"Error processing scheduled event for Appointment : {ex.Message}"
                    );
                }
            }
        }

        private static async Task<string> GetDataFromDb(
            string query,
            int parameterValue,
            SqlConnection connection
        )
        {
            string data = string.Empty;

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", parameterValue);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    data = reader.GetString(0);
                }
            }

            return data;
        }
    }
}
