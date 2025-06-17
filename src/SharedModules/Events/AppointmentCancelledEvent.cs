namespace shared.Events;

public class AppointmentCancelledEvent
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
}
