using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBlog.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
