using Microsoft.AspNetCore.SignalR;

namespace DeliverWholesale.Hubs
{
    public class NotificationHub : Hub
    {
        //  permet aux admins de rejoindre un groupe
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        //  optionnel: quitter le groupe
        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}