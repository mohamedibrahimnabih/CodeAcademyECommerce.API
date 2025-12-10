namespace CodeAcademyECommerce.API
{
    public class Cart
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Count { get; set; }
        public long Price { get; set; }
    }
}
