using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class CheckAppointmentDto
{
    [JsonPropertyName("appointment_id")]
    public int AppointmentId { get; set; }
}
