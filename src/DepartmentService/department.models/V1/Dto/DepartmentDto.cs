using System.Text.Json.Serialization;

namespace department.models.V1.Dto;

public class DepartmentDto
{
    [JsonPropertyName("dept_id")]
    public int DeptId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
