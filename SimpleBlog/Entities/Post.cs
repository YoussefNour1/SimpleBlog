using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // مهمة جدًا

namespace SimpleBlog.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(800)]
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public string? UserId { get; set; } // ده هيشاور على الـ Id في جدول AspNetUsers
        public string? AuthorName { get; set; } // ده هنخزن فيه اسم الكاتب (UserName)
        public string? PostImagePath { get; set; } // path للصورة

        // الـ Navigation Property للـ User
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}