using System.Text.Json.Serialization;

namespace appointment.models.V1.Dtos;

public class UpdateAppointmentRequestDto
{
    [JsonPropertyName("patient_id")]
    public int? PatientId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int? DoctorId { get; set; }

    [JsonPropertyName("appointment_datetime")]
    public DateTime? AppointmentDateTime { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonIgnore]
    public bool IsPatientIdSet { get; set; }

    [JsonIgnore]
    public bool IsDoctorIdSet { get; set; }

    [JsonIgnore]
    public bool IsAppointmentDateTimeSet { get; set; }

    [JsonIgnore]
    public bool IsNotesSet { get; set; }
}
