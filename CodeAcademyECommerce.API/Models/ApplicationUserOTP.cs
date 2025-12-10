namespace CodeAcademyECommerce.API
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        
        public string OTP { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAT { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddMinutes(30);
    }
}
