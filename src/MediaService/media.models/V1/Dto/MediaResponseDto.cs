using System.Text.Json.Serialization;

namespace media.models.V1.Dto;

public class MediaResponseDto
{
    [JsonPropertyName("media_id")]
    public int MediaId { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("file_url")]
    public string FileUrl { get; set; } = string.Empty;

    [JsonPropertyName("file_url_with_sas")]
    public string FileUrlWithSas { get; set; } = string.Empty;

    [JsonPropertyName("upload_date")]
    public DateTime UploadDate { get; set; }

    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = string.Empty;
}
