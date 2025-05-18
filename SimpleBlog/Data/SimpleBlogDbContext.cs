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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations(true));

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(u => u.Id) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
