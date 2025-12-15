namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class CategoryUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
