using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace patient.models.V1.Dto;

public class CreatePatientDto
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
    [JsonPropertyName("date_of_birth")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [JsonPropertyName("gender")]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("contact_number")]
    [StringLength(15)]
    public string ContactNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
}
