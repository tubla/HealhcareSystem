using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class AppointmentDto
{
    [JsonPropertyName("appointment_id")]
    public int AppointmentID { get; set; }

    [JsonPropertyName("patient_id")]
    public int PatientID { get; set; }

    [JsonPropertyName("doctor_id")]
    public int DoctorID { get; set; }

    [JsonPropertyName("appointment_date_time")]
    public DateTime AppointmentDateTime { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
