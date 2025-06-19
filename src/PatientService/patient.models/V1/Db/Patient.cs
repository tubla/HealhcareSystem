using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace patient.models.V1.Db;

[Table("patients")]
public class Patient
{
    [Key]
    [Column("patient_id")]
    public int PatientId { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("dob")]
    public DateTime Dob { get; set; }

    [Required]
    [Column("gender")]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("phone")]
    [StringLength(10)]
    public string Phone { get; set; } = string.Empty;

    [Column("address")]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Column("insurance_provider_id")]
    public int? InsuranceProviderId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }
}
