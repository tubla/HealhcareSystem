using System.Text.Json.Serialization;

namespace doctor.models.V1.Dto;

public class DepartmentResponseDto
{
    [JsonPropertyName("dept_id")]
    public int DeptId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
