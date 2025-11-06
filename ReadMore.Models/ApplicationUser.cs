using Microsoft.AspNetCore.Identity;

namespace ReadMore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty; // default waarde toegevoegd
    }
}
