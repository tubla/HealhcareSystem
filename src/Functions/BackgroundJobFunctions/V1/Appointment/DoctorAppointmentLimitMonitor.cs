using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using shared.V1.HelperClasses.Hubs;

namespace BackgroundJobFunctions.V1.Appointment;

public class DoctorAppointmentLimitMonitor(IHubContext<AppointmentHub> _hubContext,
            IConfiguration _configuration,
            ILogger<DoctorAppointmentLimitMonitor> _logger)
{
    private const int _maxAppointmentsPerDay = 10;

    [Function("DoctorAppointmentLimitMonitor")]
    public async Task Run([TimerTrigger("0 0 */1 * * *")] TimerInfo timer)
    {
        _logger.LogInformation("DoctorAppointmentLimitMonitor function started at {Time}", DateTime.UtcNow);

        var sqlConnectionString = _configuration["SqlConnection"]; // from configuration -> check Program.cs
        using var connection = new SqlConnection(sqlConnectionString);
        await connection.OpenAsync();

        var appointmentCounts = new List<(int DoctorId, DateTime SlotDate, int AppointmentCount)>();

        // Fetch appointment counts per doctor per date
        var query = @"
                    SELECT 
                        a.doctor_id,
                        CAST(a.appointment_datetime AS DATE) AS slot_date,
                        COUNT(*) AS appointment_count
                    FROM healthcare.appointment a
                    WHERE a.status != 'Cancelled'
                    GROUP BY a.doctor_id, CAST(a.appointment_datetime AS DATE)";

        using (var command = new SqlCommand(query, connection))
        {
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                appointmentCounts.Add((
                    reader.GetInt32(0),
                    reader.GetDateTime(1),
                    reader.GetInt32(2)
                ));
            }
        }

        foreach (var count in appointmentCounts)
        {
            string? previousStatus = null;
            byte[]? rowVersion = null;

            // Check existing slot
            var selectQuery = @"
                        SELECT slot_status, row_version
                        FROM healthcare.doctor_slot_availability
                        WHERE doctor_id = @DoctorId AND slot_date = @SlotDate";

            using (var selectCommand = new SqlCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@DoctorId", count.DoctorId);
                selectCommand.Parameters.AddWithValue("@SlotDate", count.SlotDate);

                using (var reader = await selectCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        previousStatus = reader.GetString(0);
                        rowVersion = (byte[])reader.GetValue(1);
                    }
                }
            }

            var newStatus = count.AppointmentCount >= _maxAppointmentsPerDay ? "Full" : "Available";

            // Insert or update slot
            var upsertQuery = previousStatus == null
                ? @"
                            INSERT INTO healthcare.doctor_slot_availability (doctor_id, slot_date, appointment_count, slot_status)
                            VALUES (@DoctorId, @SlotDate, @AppointmentCount, @SlotStatus)"
                : @"
                            UPDATE healthcare.doctor_slot_availability
                            SET appointment_count = @AppointmentCount, slot_status = @SlotStatus
                            WHERE doctor_id = @DoctorId AND slot_date = @SlotDate AND row_version = @RowVersion";

            using (var upsertCommand = new SqlCommand(upsertQuery, connection))
            {
                upsertCommand.Parameters.AddWithValue("@DoctorId", count.DoctorId);
                upsertCommand.Parameters.AddWithValue("@SlotDate", count.SlotDate);
                upsertCommand.Parameters.AddWithValue("@AppointmentCount", count.AppointmentCount);
                upsertCommand.Parameters.AddWithValue("@SlotStatus", newStatus);
                if (rowVersion != null)
                    upsertCommand.Parameters.AddWithValue("@RowVersion", rowVersion);

                await upsertCommand.ExecuteNonQueryAsync();
            }

            // Notify UI if status changed
            if (newStatus != previousStatus)
            {
                await _hubContext.Clients.All.SendAsync("NotifySlotStatus", count.DoctorId, count.SlotDate, newStatus);
                _logger.LogInformation("Notified UI of slot status change for DoctorId {DoctorId} on {SlotDate} to {SlotStatus}", count.DoctorId, count.SlotDate, newStatus);
            }
        }

        await connection.CloseAsync();

        _logger.LogInformation("DoctorAppointmentLimitMonitor function completed at {Time}", DateTime.UtcNow);
    }
}
