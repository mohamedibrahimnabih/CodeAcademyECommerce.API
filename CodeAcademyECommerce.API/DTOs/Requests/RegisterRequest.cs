using System.ComponentModel.DataAnnotations;

namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class RegisterRequest
    {
        [RegularExpression("[A-Za-z]*$", ErrorMessage = "English Char is only Allowed")]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Length(8, 32)]
        public string Password { get; set; }
        [Compare(nameof(Password)), DataType(DataType.Password), Length(8, 32)]
        public string ConfirmPassword { get; set; }
    }

}
