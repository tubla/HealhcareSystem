using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class UpdateDoctorDto
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("specialization")]
    public string? Specialization { get; set; }

    [JsonPropertyName("contact_number")]
    public string? ContactNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("hospital_affiliation")]
    public string? HospitalAffiliation { get; set; }

    [JsonIgnore]
    public bool IsFirstNameSet { get; set; }

    [JsonIgnore]
    public bool IsLastNameSet { get; set; }

    [JsonIgnore]
    public bool IsSpecializationSet { get; set; }

    [JsonIgnore]
    public bool IsContactNumberSet { get; set; }

    [JsonIgnore]
    public bool IsEmailSet { get; set; }

    [JsonIgnore]
    public bool IsHospitalAffiliationSet { get; set; }
}
