using System.ComponentModel.DataAnnotations;

namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class NewPasswordRequest
    {
        public string Id { get; set; }
        [DataType(DataType.Password), Length(8, 32)]
        public string Password { get; set; }
        [Compare(nameof(Password)), DataType(DataType.Password), Length(8, 32)]
        public string ConfirmPassword { get; set; }
    }
}
