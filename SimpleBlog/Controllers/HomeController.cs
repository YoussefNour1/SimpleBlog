using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;
using System.Diagnostics;

namespace SimpleBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SimpleBlogDbContext _context;

        public HomeController(ILogger<HomeController> logger, SimpleBlogDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel
            {
                FeaturedPost = await _context.Posts
                    .OrderByDescending(p => p.PublicationDate)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(),

                LatestPosts = await _context.Posts
                    .OrderByDescending(p => p.PublicationDate)
                    .Skip(1)
                    .Take(8)
                    .Include(p => p.User)
                    .ToListAsync(),

                PopularCategories = await _context.Categories.Include(c=>c.Posts)
                    .OrderByDescending(c => c.Posts!.Count())
                    .Take(4)
                    .ToListAsync()
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
