// في ملف Controllers/NotificationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using System.Security.Claims;

namespace SimpleBlog.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly SimpleBlogDbContext _context;
        public NotificationsController(SimpleBlogDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(int pageNumber = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            int pageSize = 10;
            var query = _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.NotificationDate);
            var totalItems = await query.CountAsync();
            var notifications = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var hasMore = (pageNumber * pageSize) < totalItems;
            var unreadCount = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(new { notifications, hasMore, unreadCount });
        }

        [HttpPost("markasread/{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null || notification.UserId != userId) return NotFound();
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            var newUnreadCount = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
            return Ok(new { newUnreadCount });
        }
    }
}