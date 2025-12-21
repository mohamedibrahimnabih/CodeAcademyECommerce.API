namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class RateProductRequest
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public IFormFile? Img { get; set; }
        public int Rate { get; set; }
    }
}
