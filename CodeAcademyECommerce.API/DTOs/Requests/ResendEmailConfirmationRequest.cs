using System.ComponentModel.DataAnnotations;

namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class ResendEmailConfirmationRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }
    }
}
