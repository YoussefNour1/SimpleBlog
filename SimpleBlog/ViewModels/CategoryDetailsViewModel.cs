using SimpleBlog.Entities;

namespace SimpleBlog.ViewModels
{
    public class CategoryDetailsViewModel
    {
        public CategoryDetailsViewModel()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }
}