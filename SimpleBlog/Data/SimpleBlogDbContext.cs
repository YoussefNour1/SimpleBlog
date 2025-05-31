using Microsoft.AspNetCore.Identity;
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
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations(true));

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(u => u.Id) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // This is to make one-to-many relationship between Post and Comment and between User and Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(true);
        }
    }
}
