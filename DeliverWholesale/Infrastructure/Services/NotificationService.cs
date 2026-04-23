using DeliverWholesale.API.Hubs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Infrastructure.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task NotifyAdmins(string message, string type)
        {
            var admins = await _context.Users
                .Where(u => u.Role == Role.Admin)
                .ToListAsync();

            if (!admins.Any())
                return;

            var notifications = admins.Select(admin => new Notification
            {
                Message = message,
                Type = type,
                UserId = admin.Id,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group("Admins")
                .SendAsync("ReceiveNotification", new
                {
                    Message = message,
                    Type = type,
                    CreatedAt = DateTime.UtcNow
                });
        }
    }
}