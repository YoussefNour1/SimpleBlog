namespace SimpleBlog.ViewModels
{
    public class CommentViewModel
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public int PostId { get; set; }
    }
}
