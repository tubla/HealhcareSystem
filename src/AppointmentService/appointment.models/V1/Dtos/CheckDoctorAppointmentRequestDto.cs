using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class CheckDoctorAppointmentRequestDto
{
    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }
}
