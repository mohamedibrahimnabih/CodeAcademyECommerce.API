using Microsoft.AspNetCore.Identity;

namespace CodeAcademyECommerce.API
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
    }
}
