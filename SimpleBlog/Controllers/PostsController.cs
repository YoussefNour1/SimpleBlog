using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    public class PostsController(IMessageSender MessageSender, SimpleBlogDbContext Context) : Controller
    {
        private readonly IMessageSender _messageSender= MessageSender;
        private readonly SimpleBlogDbContext _context = Context;
        public async Task<IActionResult> Index()
        {
            var result =await _context.Posts.ToListAsync();
            var posts = result.Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title ?? "No title",
                Content = p.Content ?? "No content",
                PublicationDate = p.PublicationDate,
                Author = p.Author ?? "No author"
            }).ToList();
            _messageSender.SendMessage("youssef", "Test", "This is a test message from the PostsController.");
            return View(posts);
        }

        public async Task<IActionResult> Details(int Id)
        {
            var Result = await _context.Posts.FirstOrDefaultAsync(p => p.Id == Id);
            if (Result == null)
            {
                return NotFound();
            }
            var post = new PostViewModel
            {
                Id = Result.Id,
                Title = Result.Title?? "No title",
                Content = Result.Content?? "No Content",
                PublicationDate =Result.PublicationDate,
                Author = Result.Author?? "No Author"
            };
            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                var newPost = new Post
                {
                    Title = post.Title,
                    Content = post.Content,
                    PublicationDate = DateTime.Now,
                    Author = post.Author
                };
                _context.Posts.Add(newPost);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);
        }
    }
}
