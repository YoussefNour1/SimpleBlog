using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}
