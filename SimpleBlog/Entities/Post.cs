using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }
        [MaxLength(800, ErrorMessage = "Content cannot exceed 800 characters.")]
        public string? Content { get; set; }
        public DateTime PublicationDate { get; set; } // تاريخ النشر
        public string? Author { get; set; } // اسم الكاتب
    }
}
