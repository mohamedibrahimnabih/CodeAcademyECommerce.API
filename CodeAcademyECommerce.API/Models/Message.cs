namespace CodeAcademyECommerce.API
{
    public enum TicketStatus
    {
        InProcessing,
        Canceled,
        Completed,
        //CanceledByCustomer
    }

    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public TicketStatus TicketStatus { get; set; } = TicketStatus.InProcessing;
    }
}
