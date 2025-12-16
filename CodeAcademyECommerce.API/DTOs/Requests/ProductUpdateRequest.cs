namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class ProductUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? MainImg { get; set; }
        public List<IFormFile>? SubImages { get; set; }
        public List<string>? Colors { get; set; }
        public long Price { get; set; }
        public long Discount { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public bool Status { get; set; }
    }
}
