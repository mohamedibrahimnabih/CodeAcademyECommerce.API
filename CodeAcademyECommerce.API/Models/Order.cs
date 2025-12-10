namespace CodeAcademyECommerce.API
{
    public enum OrderStatus
    {
        Pending,
        InProcessing,
        Shipped,
        Completed,
        Canceled
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Canceled,
        Refunded
    }

    public enum TransactionType
    {
        Stripe,
        PayPal,
        Cash,
        ApplePay
    }

    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public long TotalPrice { get; set; }

        public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Pending;
        public string SessionId { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public TransactionType TransactionType { get; set; } = TransactionType.Stripe;

        public string? CarrierName { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime ShippedDate { get; set; } 
    }
}
