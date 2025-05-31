using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    public class CommentsController(UserManager<ApplicationUser> userManager, SimpleBlogDbContext ctx, IMessageSender sender) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SimpleBlogDbContext _ctx = ctx;
        private readonly IMessageSender _messageSender = sender;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CommentViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.ValidationState;
                return RedirectToAction("Details", "Posts", new { Id = comment.PostId });
            }
            var user = await _userManager.GetUserAsync(User);
            _messageSender.SendMessage("youssef", "Test", $"This is a test message from the CommentsController." +
                $"\nId: {comment.Id}\n" +
                $"Content: {comment.Content}\n" +
                $"UserId: {comment.UserId?? user.Id}\n" +
                $"PosetId: {comment.PostId}");
            var newComment = new Comment()
            {
                Content = comment.Content,
                UserId = user.Id,
                PostId = comment.PostId,
                PublicationDate = comment.PublicationDate
            };
            await _ctx.Comments.AddAsync(newComment);
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new {Id= comment.PostId});
        }
        
    }
}
