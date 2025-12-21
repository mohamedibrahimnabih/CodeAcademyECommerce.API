namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public record PasswordUpdateRequest(string currentPassword, string newPassword);
}
