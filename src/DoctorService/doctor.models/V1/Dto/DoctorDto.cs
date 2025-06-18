using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class DoctorDto
{
    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("specialization")]
    public string Specialization { get; set; } = string.Empty;

    [JsonPropertyName("license_number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [JsonPropertyName("contact_number")]
    public string ContactNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("hospital_affiliation")]
    public string HospitalAffiliation { get; set; } = string.Empty;
}
