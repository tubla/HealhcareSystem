using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class CreateAppointmentRequestDto
{

    [Required]
    [JsonPropertyName("patient_id")]
    public int PatientId { get; set; }

    [Required]
    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [Required]
    [JsonPropertyName("appointment_datetime")]
    public DateTime AppointmentDateTime { get; set; }

    [StringLength(500)]
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
