using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Categories")]
    public class CategoriesController(SimpleBlogDbContext ctx) : Controller
    {
        private readonly SimpleBlogDbContext _context = ctx;
        [HttpGet(""), HttpGet("Index")]
        public ActionResult Index()
        {
            var categories = _context.Categories.Include(c=>c.Posts).ToList();
            return View(categories);
        }

        [HttpGet("Details/{Id:int}")]
        public async Task<ActionResult> DetailsAsync(int Id)
        {
            var existingCategory = await _context.Categories.Include(c => c.Posts).FirstOrDefaultAsync(c => c.Id == Id);
            if (existingCategory == null)
            {
                TempData["ErrorMessage"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }
            var CategoryWithPosts = new CategoryDetailsViewModel()
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Posts = [.. existingCategory.Posts]
            };
            return View(CategoryWithPosts);
        }

        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Category category)
        {
            if (ModelState.IsValid)
            {
                bool nameExists = await _context.Categories.AnyAsync(c => c.Name == category.Name);
                if (nameExists)
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);
                }

                var cat = new Category
                {
                    Name = category.Name,
                };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Category '{category.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        [HttpGet("Edit/{id:int}")]
        public ActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost("Edit/{Id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int Id, Category category)
        {
            if (ModelState.IsValid) {
                var existingCategory = _context.Categories.Find(Id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                // Check if the name already exists
                bool nameExists = _context.Categories.Any(c => c.Name == category.Name && c.Id != Id);
                if (nameExists)
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);
                }

                existingCategory.Name = category.Name;
                _context.Update(existingCategory);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Category '{category.Name}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }
        }
        [HttpPost("Delete/{Id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int Id)
        {
            var existingCategory = _context.Categories.Find(Id);
            if (existingCategory != null)
            {
                _context.Categories.Remove(existingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
