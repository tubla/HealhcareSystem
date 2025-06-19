using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doctor.models.V1.Db;

[Table("doctor")]
public class Doctor
{
    [Key]
    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("license_number")]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [Column("specialization")]
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("phone")]
    [StringLength(10)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Column("dept_id")]
    public int DeptId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }
}
