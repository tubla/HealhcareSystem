using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class CreateAppointmentDto
{
    [Required]
    [JsonPropertyName("patient_id")]
    public int PatientId { get; set; }

    [Required]
    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [Required]
    [JsonPropertyName("appointment_date_time")]
    public DateTime AppointmentDateTime { get; set; }

    [Required]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled


    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
