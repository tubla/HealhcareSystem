using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace department.models.V1.Dto;

public class CreateDepartmentDto
{
    [Required]
    [JsonPropertyName("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
