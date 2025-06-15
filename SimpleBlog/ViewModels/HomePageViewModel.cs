using SimpleBlog.Entities;

namespace SimpleBlog.ViewModels
{
    public class HomePageViewModel
    {
        public Post? FeaturedPost { get; set; }
        public List<Post> LatestPosts { get; set; } = new();
        public List<Category> PopularCategories { get; set; } = new();
    }
}