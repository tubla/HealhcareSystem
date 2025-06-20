using System.ComponentModel.DataAnnotations.Schema;

namespace prescription.models.V1.Db;

[Table("prescription_media")]
public class PrescriptionMedia
{
    [Column("prescription_id")]
    public int PrescriptionId { get; set; }

    [Column("media_id")]
    public int MediaId { get; set; }

    [ForeignKey("PrescriptionId")]
    public virtual Prescription Prescription { get; set; } = null!;
}
