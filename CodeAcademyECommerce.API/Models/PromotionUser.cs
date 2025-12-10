namespace CodeAcademyECommerce.API
{
    public class PromotionUser
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; }
    }
}
