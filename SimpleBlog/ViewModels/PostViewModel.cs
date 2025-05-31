using SimpleBlog.Entities;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.ViewModels // اسم المشروع . اسم المجلد
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; } // تاريخ النشر
        public string? UserId { get; set; }
        public string? Author { get; set; } // اسم الكاتب
        public string? AuthorImage { get; set; }
        public bool IsOwner { get; set; }
        public string? PostImagePath { get; set; }
        [Display(Name = "Post Image")]
        public IFormFile? ImageFile { get; set; } // لاستقبال الملف المرفوع
        public List<Comment> Comments { get; set; } = new List<Comment>(); // قائمة التعليقات
    }
}