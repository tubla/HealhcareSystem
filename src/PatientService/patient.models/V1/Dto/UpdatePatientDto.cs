using System.Text.Json.Serialization;

namespace patient.models.V1.Dto;

public class UpdatePatientDto
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("contact_number")]
    public string? ContactNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonIgnore]
    public bool IsFirstNameSet { get; set; }

    [JsonIgnore]
    public bool IsLastNameSet { get; set; }

    [JsonIgnore]
    public bool IsDateOfBirthSet { get; set; }

    [JsonIgnore]
    public bool IsGenderSet { get; set; }

    [JsonIgnore]
    public bool IsContactNumberSet { get; set; }

    [JsonIgnore]
    public bool IsEmailSet { get; set; }

    [JsonIgnore]
    public bool IsAddressSet { get; set; }
}
