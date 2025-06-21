using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class UpdateDoctorRequestDto
{
    [JsonPropertyName("name")]
    [StringLength(100)]
    public string? Name { get; set; }

    [JsonPropertyName("license_number")]
    [StringLength(50)]
    public string? LicenseNumber { get; set; }

    [JsonPropertyName("specialization")]
    [StringLength(100)]
    public string? Specialization { get; set; }

    [JsonPropertyName("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    [StringLength(10)]
    public string? Phone { get; set; }

    [JsonPropertyName("dept_id")]
    public int? DeptId { get; set; }

    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }


    [JsonIgnore]
    public bool IsNameSet { get; set; }

    [JsonIgnore]
    public bool IsLicenseNumberSet { get; set; }

    [JsonIgnore]
    public bool IsSpecializationSet { get; set; }

    [JsonIgnore]
    public bool IsPhoneSet { get; set; }

    [JsonIgnore]
    public bool IsEmailSet { get; set; }

    [JsonIgnore]
    public bool IsDeptIdSet { get; set; }

    [JsonIgnore]
    public bool IsUserIdSet { get; set; }
}
