namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public class ReplyRequest
    {
        public int ProductId { get; set; }

        public int RatingId { get; set; }
        public string Reply { get; set; } = string.Empty;
    }
}
