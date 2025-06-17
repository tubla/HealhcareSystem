using System.ComponentModel.DataAnnotations.Schema;

namespace appointment.models.V1.Db;

[Table("appointment", Schema = "healthcare")]
public class Appointment
{
    [Column("appointment_id")]
    public int AppointmentId { get; set; }

    [Column("patient_id")]
    public int PatientId { get; set; }

    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Column("appointment_date_time")]
    public DateTime AppointmentDateTime { get; set; }

    [Column("status")]
    public string Status { get; set; } = string.Empty;

    [Column("reason")]
    public string Reason { get; set; } = string.Empty;
}
