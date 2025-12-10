namespace CodeAcademyECommerce.API
{
    public class RatingReply
    {
        public int Id { get; set; }

        public int RatingId { get; set; }
        public Rating Rating { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string Comment { get; set; }
    }
}
