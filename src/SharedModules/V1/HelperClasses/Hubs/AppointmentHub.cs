using Microsoft.AspNetCore.SignalR;

namespace shared.V1.HelperClasses.Hubs;

public class AppointmentHub : Hub
{
    public async Task NotifySlotStatus(int doctorId, DateTime slotDate, string slotStatus)
    {
        await Clients.All.SendAsync(AppointmentHubMethod.NotifySlotStatus, doctorId, slotDate, slotStatus);
    }
}
