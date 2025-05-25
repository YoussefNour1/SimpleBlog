using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBlog.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; } = DateTime.Now;
        [Required]
        public string UserId { get; set; }
        [Required]
        public int PostId { get; set; }

        // Navigation properties
        public virtual IdentityUser User { get; set; }
        public virtual Post Post { get; set; }
    }
}
