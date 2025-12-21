namespace CodeAcademyECommerce.API.DTOs.Requests
{
    public record OrderUpdateRequest(OrderStatus orderStatus, string trackingNumber, string carrierName);
}
