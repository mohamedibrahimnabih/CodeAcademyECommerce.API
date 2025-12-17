namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class UserCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool EmailConfirmation { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
