using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    
    public class PostsController(IMessageSender MessageSender, SimpleBlogDbContext Context, UserManager<IdentityUser> user) : Controller
    {
        private readonly IMessageSender _messageSender = MessageSender;
        private readonly SimpleBlogDbContext _context = Context;
        private readonly UserManager<IdentityUser> _user = user;
        public async Task<IActionResult> Index()
        {
            var result = await _context.Posts.ToListAsync();
            var posts = result.Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title ?? "No title",
                Content = p.Content ?? "No content",
                PublicationDate = p.PublicationDate,
                Author = p.AuthorName ?? "No author"
            }).OrderByDescending(p => p.PublicationDate).ToList();
            _messageSender.SendMessage("youssef", "Test", "This is a test message from the PostsController.");
            return View(posts);
        }

        public async Task<IActionResult> Details(int Id)
        {
            var Result = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == Id);
            var flag = false;
            if (User.Identity.IsAuthenticated)
            {
                var LoggedUser = await _user.GetUserAsync(User);
                var LoggedUserId = await _user.GetUserIdAsync(LoggedUser);
                flag = LoggedUserId == Result.UserId;
            }
            if (Result == null)
            {
                return NotFound();
            }
            var post = new PostViewModel
            {
                Id = Result.Id,
                Title = Result.Title ?? "No title",
                Content = Result.Content ?? "No Content",
                PublicationDate = Result.PublicationDate,
                Author = Result.User.Email ?? "No Author",
                flag = flag
            };
            return View(post);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(PostViewModel post)
        {
            var LoggedUser= await _user.GetUserAsync(User);
            var LoggedUserId = await _user.GetUserIdAsync(LoggedUser);
            var LoggedUserName = await _user.GetUserNameAsync(LoggedUser);
            if (LoggedUser == null)
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {
                var newPost = new Post
                {
                    Title = post.Title,
                    Content = post.Content,
                    PublicationDate = DateTime.Now,
                    UserId = LoggedUserId,
                    AuthorName = LoggedUserName
                };
                _context.Posts.Add(newPost);
                await _context.SaveChangesAsync();
                _messageSender.SendMessage("Admin", "DB Activity", $"New post '{newPost.Title}' created by user ID: {LoggedUserId}.");
                TempData["SuccessMessage"] = $"Post '{newPost.Title}' was created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var Result = await _context.Posts.FirstOrDefaultAsync(p => p.Id == Id);
            var LoggedUser = await _user.GetUserAsync(User);
            var LoggedUserId = await _user.GetUserIdAsync(LoggedUser);

            if (Result?.UserId != LoggedUserId)
            {
                return Forbid();
            }
            if (Result == null)
            {
                return NotFound();
            }
            var post = new PostViewModel
            {
                Id = Result.Id,
                Title = Result.Title,
                Content = Result.Content,
                PublicationDate = Result.PublicationDate,
                Author = _user.GetUserName(User) ?? "No Author"
            };
            return View(post);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel postViewModel)
        {
            var LoggedUser = await _user.GetUserAsync(User);
            var LoggedUserId = await _user.GetUserIdAsync(LoggedUser);
            var postToUpdate = await _context.Posts.FindAsync(postViewModel.Id);
            
            if (postToUpdate == null)
            {
                return NotFound();
            }
            if (LoggedUserId != postToUpdate?.UserId)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View(postViewModel);
            }
            
            postToUpdate.Title = postViewModel.Title;
            postToUpdate.Content = postViewModel.Content;
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Posts.Any(e => e.Id == postViewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Details", new { Id = postViewModel.Id });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var PostToDelete = await _context.Posts.FindAsync(Id);
            if (PostToDelete == null)
            {
                return NotFound();
            }
            var LoggedUser = await _user.GetUserAsync(User);
            var LoggedUserId = await _user.GetUserIdAsync(LoggedUser);
            if (LoggedUserId != PostToDelete?.UserId)
            {
                return Forbid();
            }
            _context.Posts.Remove(PostToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
