using SimpleBlog.Entities;

namespace SimpleBlog.ViewModels // اسم المشروع . اسم المجلد
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; } // تاريخ النشر
        public string? Author { get; set; } // اسم الكاتب
        public bool? flag { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>(); // قائمة التعليقات
    }
}