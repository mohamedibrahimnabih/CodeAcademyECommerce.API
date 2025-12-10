namespace CodeAcademyECommerce.API
{
    public class Promotion
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public long Discount { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddDays(7);
        public int MaxOfUsage { get; set; } = 100;

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
