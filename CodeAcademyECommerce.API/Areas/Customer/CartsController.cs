using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class CartsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CartsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? code)
        {
            //var user = await _userManager.GetUserAsync(User);

            //if (user == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var carts = _context.Carts.Include(e => e.Product).Where(e => e.ApplicationUserId == userId);

            if (code is not null)
            {
                var promotion = _context.Promotions.FirstOrDefault(e => e.Code == code && e.MaxOfUsage > 0 && e.ValidTo > DateTime.UtcNow);

                if (promotion is null)
                    return BadRequest(new
                    {
                        key = "CodeNotValid",
                        msg = "Code Not Valid"
                    });
                else
                {
                    var cartsInDb = _context.Carts.Where(e => e.ApplicationUserId == userId).ToList();
                    bool isFounded = false;
                    foreach (var item in cartsInDb)
                    {
                        if (item.ProductId == promotion.ProductId)
                        {
                            isFounded = true;

                            var promotionUsers = _context.PromotionUsers.Where(e => e.ApplicationUserId == userId).ToList();

                            bool isUsed = false;
                            foreach (var promotionUser in promotionUsers)
                            {
                                if (promotionUser.PromotionId == promotion.Id)
                                {
                                    isUsed = true;

                                    return BadRequest(new
                                    {
                                        key = "CodeNotValid",
                                        msg = "Code Not Valid"
                                    });
                                }
                            }

                            if (!isUsed)
                            {
                                item.Price -= promotion.Discount;
                                promotion.MaxOfUsage -= 1;
                                _context.PromotionUsers.Add(new()
                                {
                                    ApplicationUserId = userId,
                                    PromotionId = promotion.Id
                                });
                                _context.SaveChanges();

                                //return Ok(new
                                //{
                                //    success_msg = "Code Valid",
                                //    trace_id = Guid.NewGuid().ToString(),
                                //    date = DateTime.Now
                                //});
                            }

                        }
                    }

                    if (!isFounded)
                        return BadRequest(new
                        {
                            key = "CodeNotValid",
                            msg = "Code Not Valid"
                        });
                }
            }

            return Ok(carts.ToList());
        }

        [HttpPost]
        public IActionResult AddToCart(CartCreateRequest cartCreateRequest)
        {
            //var user = await _userManager.GetUserAsync(User);

            //if (user == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var product = _context.Products.FirstOrDefault(e => e.Id == cartCreateRequest.productId);

            if (product == null) return NotFound();

            var cartInDb = _context.Carts.FirstOrDefault(e => e.ProductId == cartCreateRequest.productId && e.ApplicationUserId == userId);

            if (cartInDb is not null)
            {
                cartInDb.Count += cartCreateRequest.count;
            }
            else
            {
                _context.Carts.Add(new()
                {
                    ApplicationUserId = userId,
                    Count = cartCreateRequest.count,
                    ProductId = cartCreateRequest.productId,
                    Price = product.Price
                });
            }

            _context.SaveChanges();

            return Created();
        }

        [HttpGet("{id}/IncrementItem")]
        public IActionResult IncrementItem(int id)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.Id == id);
            if (cart is null) return NotFound();

            cart.Count += 1;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}/DecrementItem")]
        public IActionResult DecrementItem(int id)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.Id == id);
            if (cart is null) return NotFound();

            if (cart.Count > 1)
            {
                cart.Count -= 1;
                _context.SaveChanges();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.Id == id);

            if (cart is null) return NotFound();

            _context.Carts.Remove(cart);
            _context.SaveChanges();
            
            return NoContent();
        }

        [HttpGet("Pay")]
        public async Task<IActionResult> Pay()
        {

            //var user = await _userManager.GetUserAsync(User);

            //if (user == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var carts = _context.Carts.Include(e => e.Product).Where(e => e.ApplicationUserId == userId);

            Order order = new()
            {
                ApplicationUserId = userId,
                TotalPrice = carts.Sum(e => e.Price * e.Count),
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkouts/Success/{order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkouts/Cancel/{order.Id}",
            };

            foreach (var item in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "omr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        },
                        UnitAmount = (item.Price * 1000),
                    },
                    Quantity = item.Count,
                });
            }


            var service = new SessionService();
            var session = service.Create(options);

            order.SessionId = session.Id;
            _context.SaveChanges();

            return Ok(new
            {
                url = session.Url
            });
        }
    }
}
