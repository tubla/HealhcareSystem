using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace prescription.models.V1.Dto;

public class MediaDto
{
    [JsonPropertyName("media_id")]
    public int MediaId { get; set; }

    [JsonPropertyName("file_name")]
    [StringLength(100)]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("file_url")]
    [StringLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [JsonPropertyName("upload_date")]
    public DateTime UploadDate { get; set; }

    [JsonPropertyName("content_type")]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;
}
