using System.ComponentModel.DataAnnotations;

namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class LoginRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }
        [DataType(DataType.Password), Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
