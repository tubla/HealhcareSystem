using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace patient.models.V1.Dto;

public class CreatePatientDto
{
    [Required]
    [JsonPropertyName("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("dob")]
    public DateTime Dob { get; set; }

    [Required]
    [JsonPropertyName("gender")]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("phone")]
    [StringLength(10)]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("insurance_provider_id")]
    public int? InsuranceProviderId { get; set; }

    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }
}
