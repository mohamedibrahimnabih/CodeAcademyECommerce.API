namespace CodeAcademyECommerce.API
{
    public class Product : AuditLogging
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public bool Status { get; set; }
        public long Price { get; set; }
        public long Discount { get; set; }
        public double Rate { get; set; }
        public int Traffic { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = default!;
    }
}
