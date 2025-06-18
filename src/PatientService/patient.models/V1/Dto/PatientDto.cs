using System.Text.Json.Serialization;

namespace patient.models.V1.Dto;

public class PatientDto
{
    [JsonPropertyName("patient_id")]
    public int PatientId { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("date_of_birth")]
    public DateTime DateOfBirth { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("contact_number")]
    public string ContactNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
}
