using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prescription.models.V1.Db;

[Table("prescription")]
public class Prescription
{
    [Key]
    [Column("prescription_id")]
    public int PrescriptionId { get; set; }

    [Required]
    [Column("appointment_id")]
    public int AppointmentId { get; set; }

    [Required]
    [Column("medication_id")]
    public int MedicationId { get; set; }

    [Required]
    [Column("dosage")]
    [StringLength(100)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [Column("duration")]
    [StringLength(50)]
    public string Duration { get; set; } = string.Empty;

    [Required]
    [Column("issue_date")]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<PrescriptionMedia> PrescriptionMedia { get; set; } = new List<PrescriptionMedia>();
}
