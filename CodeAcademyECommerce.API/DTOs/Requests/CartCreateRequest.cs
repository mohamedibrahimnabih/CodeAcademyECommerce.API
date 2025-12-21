namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public record CartCreateRequest(int productId, int count = 1);
}
