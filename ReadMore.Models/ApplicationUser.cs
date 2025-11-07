using Microsoft.AspNetCore.Identity;

namespace ReadMore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Role { get; set; }

        public bool IsAdmin => Role != null && Role.ToLower() == "admin";
    }
}
