using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class CreateDoctorRequestDto
{
    [Required]
    [JsonPropertyName("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("license_number")]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("specialization")]
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("phone")]
    [StringLength(10)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("dept_id")]
    public int DeptId { get; set; }

    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }
}
