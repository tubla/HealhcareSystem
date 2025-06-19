using System.Text.Json.Serialization;

namespace department.models.V1.Dto;

public class UpdateDepartmentDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore]
    public bool IsNameSet { get; set; }
}
