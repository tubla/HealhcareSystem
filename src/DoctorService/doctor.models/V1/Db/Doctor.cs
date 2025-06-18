using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doctor.models.V1.Db;

[Table("doctors")]
public class Doctor
{
    [Key]
    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Required]
    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("specialization")]
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [Column("license_number")]
    [StringLength(20)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Column("contact_number")]
    [StringLength(15)]
    public string ContactNumber { get; set; } = string.Empty;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("hospital_affiliation")]
    [StringLength(100)]
    public string HospitalAffiliation { get; set; } = string.Empty;
}
