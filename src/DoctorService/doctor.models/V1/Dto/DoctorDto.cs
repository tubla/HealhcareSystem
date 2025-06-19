using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class DoctorDto
{
    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("license_number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [JsonPropertyName("specialization")]
    public string Specialization { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("dept_id")]
    public int DeptId { get; set; }

    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }
}
