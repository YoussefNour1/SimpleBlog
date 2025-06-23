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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

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

            // many-to-many relationship between Post and Category  # No need for this in the current design
            //modelBuilder.Entity<Post>()
            //    .HasMany(p => p.Categories)
            //    .WithMany(c => c.Posts)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "PostCategory",
            //        j => j
            //            .HasOne<Category>()
            //            .WithMany()
            //            .HasForeignKey("CategoryId")
            //            .HasPrincipalKey(c => c.Id)
            //            .OnDelete(DeleteBehavior.Cascade),
            //        j => j
            //            .HasOne<Post>()
            //            .WithMany()
            //            .HasForeignKey("PostId")
            //            .HasPrincipalKey(p => p.Id)
            //            .OnDelete(DeleteBehavior.Cascade));
        }
    }
}
