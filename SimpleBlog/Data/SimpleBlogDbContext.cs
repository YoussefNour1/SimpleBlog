using Microsoft.EntityFrameworkCore;
using SimpleBlog.Entities;

namespace SimpleBlog.Data
{
    public class SimpleBlogDbContext : DbContext
    {
        public SimpleBlogDbContext(DbContextOptions<SimpleBlogDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

    }
}
