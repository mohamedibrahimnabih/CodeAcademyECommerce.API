using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Customer
{
    [Authorize]
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER_AREA)]
    public class CheckoutsController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CheckoutsController(IEmailSender emailSender, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Success(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var order = _context.Orders.FirstOrDefault(e => e.Id == id);
            if (order == null) return NotFound();

            // Send Mail
            await _emailSender.SendEmailAsync(user.Email!, "Place Order Successfully",
                $"<h1>Thanks to complete your order At {order.CreatedAt}, order id is: {id}, Total Price: {order.TotalPrice}</h1>");

            // Update Order Status
            var service = new SessionService();
            var transaction = service.Get(order.SessionId);
            order.OrderStatus = OrderStatus.InProcessing;
            order.TransactionStatus = TransactionStatus.Completed;
            order.TransactionId = transaction.PaymentIntentId;

            // Create Order Items
            var carts = _context.Carts.Include(e => e.Product).Where(e => e.ApplicationUserId == user.Id);

            foreach (var item in carts)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    OrderId = id,
                    Price = item.Price,
                });
            }

            // Decrease Product Quantity
            foreach (var item in carts)
                item.Product.Quantity -= item.Count;

            // Delete Cart
            _context.Carts.RemoveRange(carts);

            // Commit
            _context.SaveChanges();

            return Ok();
        }
    }
}
