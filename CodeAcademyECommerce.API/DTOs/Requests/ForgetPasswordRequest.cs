using System.ComponentModel.DataAnnotations;

namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class ForgetPasswordRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }
    }
}
