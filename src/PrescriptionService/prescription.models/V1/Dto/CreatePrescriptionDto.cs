using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace prescription.models.V1.Dto;

public class CreatePrescriptionDto
{
    [Required]
    [JsonPropertyName("appointment_id")]
    public int AppointmentId { get; set; }

    [Required]
    [JsonPropertyName("medication_id")]
    public int MedicationId { get; set; }

    [Required]
    [JsonPropertyName("dosage")]
    [StringLength(100)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("duration")]
    [StringLength(50)]
    public string Duration { get; set; } = string.Empty;
}
