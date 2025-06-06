﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Entities;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{

    public class PostsController(IMessageSender MessageSender, SimpleBlogDbContext Context, UserManager<ApplicationUser> user, IWebHostEnvironment webHost) : Controller
    {
        private readonly IMessageSender _messageSender = MessageSender;
        private readonly SimpleBlogDbContext _context = Context;
        private readonly UserManager<ApplicationUser> _user = user;
        private readonly IWebHostEnvironment _host = webHost;
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var PostsPaginated = new PostsViewModel
            {
                PageIndex = pageNumber,
                PageSize = 6,
                TotalCount = await _context.Posts.CountAsync()
            };
            var result = await _context.Posts
                .Include(p=> p.User)
                .OrderByDescending(p=> p.PublicationDate)
                .Skip((pageNumber - 1) * PostsPaginated.PageSize)
                .Take(PostsPaginated.PageSize)
                .ToListAsync();

            var posts = result.Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title ?? "No title",
                Content = p.Content ?? "No content",
                PublicationDate = p.PublicationDate,
                Author = p.AuthorName ?? "No author",
                PostImagePath = p.PostImagePath
            }).OrderByDescending(p => p.PublicationDate).ToList();
            PostsPaginated.Posts = posts;
            _messageSender.SendMessage("youssef", "Test", "This is a test message from the PostsController.");
            return View(PostsPaginated);
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
                AuthorImage = Result.User?.ProfileImagePath ?? "/images/profiles/default-avatar.png",
                Author = Result.User.DisplayName ?? Result.User.Email,
                Comments = [.. _context.Comments.Where(c => c.PostId == Result.Id).Include(c=> c.User)],
                IsOwner = flag,
                PostImagePath = Result.PostImagePath
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
                string? UniqueFileName = await ProcessUploadedFile(post.ImageFile);
                var newPost = new Post
                {
                    Title = post.Title,
                    Content = post.Content,
                    PublicationDate = DateTime.Now,
                    UserId = LoggedUserId,
                    AuthorName = LoggedUserName,
                    PostImagePath = UniqueFileName != null ? $"{UniqueFileName}" : null
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

            if (Result?.UserId != LoggedUserId && !User.IsInRole("Admin"))
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
                Author = Result.AuthorName ?? "No Author",
                PostImagePath = Result.PostImagePath 
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
            if (LoggedUserId != postToUpdate?.UserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View(postViewModel);
            }
            if (postViewModel.ImageFile != null && postViewModel.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(postToUpdate.PostImagePath))
                {
                    string oldImageServerPath = Path.Combine(_host.WebRootPath, postToUpdate.PostImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImageServerPath))
                    {
                        System.IO.File.Delete(oldImageServerPath);
                    }
                }
                postToUpdate.PostImagePath = await ProcessUploadedFile(postViewModel.ImageFile);
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
            if (LoggedUserId != PostToDelete?.UserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            _context.Posts.Remove(PostToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<string?> ProcessUploadedFile(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null; 
            }
            string uploadsFolder = Path.Combine(_host.WebRootPath, "images", "posts");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                return $"/images/posts/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _messageSender.SendMessage("ErrorLog", "File Upload Error", ex.Message);
                return null;
            }
        }
    }
}
