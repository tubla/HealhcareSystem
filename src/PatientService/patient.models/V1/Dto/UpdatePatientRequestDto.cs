using System.Text.Json.Serialization;

namespace patient.models.V1.Dto;

public class UpdatePatientRequestDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("dob")]
    public DateTime Dob { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("insurance_provider_id")]
    public int? InsuranceProviderId { get; set; }

    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }


    [JsonIgnore]
    public bool IsNameSet { get; set; }

    [JsonIgnore]
    public bool IsDobSet { get; set; }

    [JsonIgnore]
    public bool IsGenderSet { get; set; }

    [JsonIgnore]
    public bool IsEmailSet { get; set; }

    [JsonIgnore]
    public bool IsPhoneSet { get; set; }

    [JsonIgnore]
    public bool IsAddressSet { get; set; }

    [JsonIgnore]
    public bool IsInsuranceProviderIdSet { get; set; }

    [JsonIgnore]
    public bool IsUserIdSet { get; set; }
}
