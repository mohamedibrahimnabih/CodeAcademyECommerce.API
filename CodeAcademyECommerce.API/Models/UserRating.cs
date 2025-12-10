namespace CodeAcademyECommerce.API
{
    public class UserRating
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int RatingId { get; set; }
        public Rating Rating { get; set; }
    }
}
