using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class CreateDoctorDto
{
    [Required]
    [JsonPropertyName("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("specialization")]
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("license_number")]
    [StringLength(20)]
    public string LicenseNumber { get; set; } = string.Empty;

    [JsonPropertyName("contact_number")]
    [StringLength(15)]
    public string ContactNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("hospital_affiliation")]
    [StringLength(100)]
    public string HospitalAffiliation { get; set; } = string.Empty;
}
