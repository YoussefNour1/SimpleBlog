using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
        public string Name { get; set; }

        // Navigation property for related posts
        public ICollection<Post> Posts { get; set; } = [];

        // Override ToString for easier debugging
        public override string ToString() => $"{Name}";
    }
}
