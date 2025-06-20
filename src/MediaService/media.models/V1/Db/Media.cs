using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace media.models.V1.Db;

[Table("media", Schema = "healthcare")]
public class Media
{
    [Key]
    [Column("media_id")]
    public int MediaId { get; set; }

    [Required]
    [Column("file_name")]
    [StringLength(100)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [Column("file_url")]
    [StringLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [Required]
    [Column("upload_date")]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("content_type")]
    [StringLength(50)]
    public string ContentType { get; set; } = string.Empty;
}
