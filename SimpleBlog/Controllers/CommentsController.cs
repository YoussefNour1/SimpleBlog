using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using SimpleBlog.Hubs;
using System.Security.Claims;

namespace SimpleBlog.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly SimpleBlogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        // Constructor remains the same...
        public CommentsController(SimpleBlogDbContext context, UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> hubContext, ILogger<CommentsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postId, string content)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound();
            var commenterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var commenter = await _userManager.FindByIdAsync(commenterId);
            if (commenter == null || string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", "Posts", new { id = postId });
            }

            var comment = new Comment
            {
                Content = content,
                PublicationDate = DateTime.Now,
                PostId = postId,
                UserId = commenterId
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(); // Save comment first to get its ID

            if (post.UserId != commenterId)
            {
                var notification = new Notification
                {
                    Message = $"{commenter.DisplayName ?? commenter.UserName} commented on your post: \"{post.Title}\"",
                    UserId = post.UserId,
                    // ====[ Adding a unique signature to the URL ]====
                    Url = Url.Action("Details", "Posts", new { id = postId }, Request.Scheme) + $"#comment-{comment.Id}",
                    IsRead = false,
                    NotificationDate = comment.PublicationDate
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(post.UserId)
                                 .SendAsync("ReceiveNotification", notification.Message, notification.Url);
            }

            return RedirectToAction("Details", "Posts", new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            var postId = comment.Post.Id;

            // ====[ Find and delete the related notification ]====
            // 1. Create the unique signature to search for
            var commentUrlFragment = $"#comment-{comment.Id}";

            // 2. Find the notification using the signature in the URL
            var notificationToDelete = await _context.Notifications
                                               .FirstOrDefaultAsync(n => n.Url != null && n.Url.EndsWith(commentUrlFragment));

            string? notificationOwnerId = null;

            if (notificationToDelete != null)
            {
                notificationOwnerId = notificationToDelete.UserId;
                _context.Notifications.Remove(notificationToDelete);
            }

            // 3. Delete the comment itself
            _context.Comments.Remove(comment);

            // 4. Save all changes
            await _context.SaveChangesAsync();

            // 5. Send real-time signal to update the UI
            if (notificationOwnerId != null)
            {
                await _hubContext.Clients.User(notificationOwnerId).SendAsync("RefreshNotifications");
            }

            TempData["SuccessMessage"] = "Comment deleted successfully.";
            return RedirectToAction("Details", "Posts", new { id = postId });
        }
    }
}
