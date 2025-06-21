using System.Text.Json.Serialization;

namespace prescription.models.V1.Dto;

public class PrescriptionResponseDto
{
    [JsonPropertyName("prescription_id")]
    public int PrescriptionId { get; set; }

    [JsonPropertyName("appointment_id")]
    public int AppointmentId { get; set; }

    [JsonPropertyName("medication_id")]
    public int MedicationId { get; set; }

    [JsonPropertyName("dosage")]
    public string Dosage { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public string Duration { get; set; } = string.Empty;

    [JsonPropertyName("issue_date")]
    public DateTime IssueDate { get; set; }

    [JsonPropertyName("media")]
    public IEnumerable<MediaResponseDto> Media { get; set; } = new List<MediaResponseDto>();
}
