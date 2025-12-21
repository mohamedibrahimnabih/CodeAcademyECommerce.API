using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Customer
{
    [Authorize]
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER_AREA)]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll(int? orderId, int page = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var orders = _context.Orders.Where(e => e.ApplicationUserId == userId);

            if (orderId is not null)
                orders = orders.Where(e => e.Id == orderId);

            double totalPages = Math.Ceiling(orders.Count() / 5.0);
            int currentPage = page;
            orders = orders.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                totalPages,
                currentPage,
                orders
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);

            //if (user is null) return NotFound();

            var orderDetails = _context.Orders.Include(e => e.ApplicationUser).FirstOrDefault(e => e.Id == id && e.ApplicationUserId == userId);

            var items = _context.OrderItems.Include(e => e.Product).Include(e => e.Order).Where(e => e.OrderId == id);

            var userRatings = _context.Ratings.Where(e => e.ApplicationUserId == userId && items.Select(e => e.ProductId).Contains(e.ProductId));

            return Ok(new 
            {
                orderDetails,
                items,
                userRatings
            });
        }

        [HttpGet("{id}/Refund")]
        public IActionResult Refund(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);

            //if (user is null) return NotFound();

            var orderDetails = _context.Orders.FirstOrDefault(e => e.Id == id && e.ApplicationUserId == userId);

            if (orderDetails is null) return NotFound();

            var options = new RefundCreateOptions
            {
                PaymentIntent = orderDetails.TransactionId,
                Amount = orderDetails.TotalPrice * 1000,
                Reason = RefundReasons.Unknown
            };

            var service = new RefundService();
            var session = service.Create(options);

            orderDetails.OrderStatus = OrderStatus.Canceled;
            orderDetails.TransactionStatus = TransactionStatus.Refunded;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}/ReOrder")]
        public IActionResult ReOrder(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);

            //if (user is null) return NotFound();

            var order = _context.Orders.FirstOrDefault(e => e.Id == id && e.ApplicationUserId == userId);

            if (order is null) return NotFound();

            var items = _context.OrderItems.Include(e => e.Product).Where(e => e.OrderId == id).ToList();

            var cartInDb = _context.Carts.Where(e => e.ApplicationUserId == userId).ToArray();

            foreach (var item in items)
            {
                bool isFounded = false;

                foreach (var cart in cartInDb)
                {
                    if (item.ProductId == cart.ProductId)
                    {
                        cart.Count += item.Count;
                        isFounded = true;
                    }
                }

                if (!isFounded)
                {
                    _context.Carts.Add(new()
                    {
                        ApplicationUserId = userId,
                        ProductId = item.ProductId,
                        Count = item.Count,
                        Price = item.Price,
                    });
                }
            }

            _context.SaveChanges();

            return Created();
        }

        [HttpGet("{id}/RateProduct")]
        public IActionResult RateProduct(int id)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(e => e.Id == id);
            if (orderItem == null) return NotFound();
            return Ok(orderItem);
        }

        [HttpPost]
        public IActionResult RateProduct(RateProductRequest rateProductRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);

            //if (user is null) return NotFound();


            var orderItem = _context.OrderItems.Include(e => e.Product).FirstOrDefault(e => e.Id == rateProductRequest.Id);
            if (orderItem == null) return NotFound();

            var newRate = new Rating()
            {
                ApplicationUserId = userId,
                ProductId = orderItem.ProductId,
                Rate = rateProductRequest.Rate,
                Comment = rateProductRequest.Comment ?? "",
            };

            if (rateProductRequest.Img is not null && rateProductRequest.Img.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(rateProductRequest.Img.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(rateProductRequest.Img.FileName)}";

                newRate.Img = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\ratings", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    rateProductRequest.Img.CopyTo(stream);
                }
            }

            _context.Ratings.Add(newRate);
            _context.SaveChanges();

            var avgRate = _context.Ratings.Where(e => e.ProductId == orderItem.ProductId).Average(e => e.Rate);

            orderItem.Product.Rate = avgRate;
            _context.SaveChanges();

            //return RedirectToAction("Details", "Home", new { area = "Customer", id = orderItem.ProductId });
            //return Redirect($"{Request.Scheme}://{Request.Host}/Customer/Home/Details/{orderItem.ProductId}#comments");
            return Created($"{Request.Scheme}://{Request.Host}/Customer/Home/Details/{orderItem.ProductId}#comments", new
            {
                sucess_msg = "Add Rate Successfully"
            });
        }
    }
}
