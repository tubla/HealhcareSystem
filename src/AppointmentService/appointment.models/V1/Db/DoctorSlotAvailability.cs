using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appointment.models.V1.Db;

[Table("doctor_slot_availability", Schema = "healthcare")]
public class DoctorSlotAvailability
{
    [Key]
    [Column("slot_id")]
    public int SlotId { get; set; }

    [Required]
    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Required]
    [Column("slot_date")]
    public DateTime SlotDate { get; set; }

    [Required]
    [Column("appointment_count")]
    public int AppointmentCount { get; set; }

    [Required]
    [Column("slot_status")]
    [StringLength(20)]
    public string SlotStatus { get; set; } = "Available";

    [Timestamp]
    [Column("row_version")]
    public byte[] RowVersion { get; set; } = null!;
}
