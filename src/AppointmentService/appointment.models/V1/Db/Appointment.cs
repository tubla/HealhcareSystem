using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appointment.models.V1.Db;

[Table("appointment", Schema = "healthcare")]
public class Appointment
{
    [Key]
    [Column("appointment_id")]
    public int AppointmentId { get; set; }

    [Required]
    [Column("patient_id")]
    public int PatientId { get; set; }

    [Required]
    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Required]
    [Column("appointment_date_time")]
    public DateTime AppointmentDateTime { get; set; }

    [Required]
    [Column("status")]
    public string Status { get; set; } = string.Empty;

    [Column("reason")]
    public string? Reason { get; set; }
}
