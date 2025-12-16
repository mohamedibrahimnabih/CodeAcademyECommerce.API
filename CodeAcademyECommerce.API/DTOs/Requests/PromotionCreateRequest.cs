namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class PromotionCreateRequest
    {
        public string Code { get; set; }
        public long Discount { get; set; }
        public int MaxOfUsage { get; set; }

        public int ProductId { get; set; }
    }
}
