using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace prescription.models.V1.Dto;

public class UpdatePrescriptionDto
{
    [JsonPropertyName("appointment_id")]
    public int? AppointmentId { get; set; }

    [JsonPropertyName("medication_id")]
    public int? MedicationId { get; set; }

    [JsonPropertyName("dosage")]
    [StringLength(100)]
    public string? Dosage { get; set; }

    [JsonPropertyName("duration")]
    [StringLength(50)]
    public string? Duration { get; set; }

    [JsonIgnore]
    public bool IsAppointmentIdSet { get; set; }

    [JsonIgnore]
    public bool IsMedicationIdSet { get; set; }

    [JsonIgnore]
    [StringLength(100)]
    public bool IsDosageSet { get; set; }

    [JsonIgnore]
    [StringLength(50)]
    public bool IsDurationSet { get; set; }
}
