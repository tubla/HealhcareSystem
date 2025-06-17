using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class AppointmentDto
{
    [JsonPropertyName("appointment_id")]
    public int AppointmentId { get; set; }

    [JsonPropertyName("patient_id")]
    public int PatientId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("appointment_date_time")]
    public DateTime AppointmentDateTime { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
