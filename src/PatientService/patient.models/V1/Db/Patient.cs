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
    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("date_of_birth")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Column("gender")]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;

    [Column("contact_number")]
    [StringLength(10)]
    public string ContactNumber { get; set; } = string.Empty;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("address")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
}
