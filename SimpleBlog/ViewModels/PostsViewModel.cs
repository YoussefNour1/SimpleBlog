namespace SimpleBlog.ViewModels
{
    public class PostsViewModel: PaginationViewModel
    {
        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}
