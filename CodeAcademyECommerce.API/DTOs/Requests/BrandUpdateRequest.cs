namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class BrandUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
