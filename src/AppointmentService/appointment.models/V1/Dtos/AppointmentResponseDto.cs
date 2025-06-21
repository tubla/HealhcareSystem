using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class AppointmentResponseDto
{
    [JsonPropertyName("appointment_id")]
    public int AppointmentId { get; set; }

    [JsonPropertyName("patient_id")]
    public int PatientId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("appointment_datetime")]
    public DateTime AppointmentDateTime { get; set; }

    [JsonPropertyName("status")]
    [StringLength(50)]
    public string Status { get; set; } = "Scheduled";

    [JsonPropertyName("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }
}
