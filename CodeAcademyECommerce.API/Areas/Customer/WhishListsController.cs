using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Customer
{
    [Authorize]
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER_AREA)]
    public class WhishListsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public WhishListsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //var user = await _userManager.GetUserAsync(User);

            //if (user == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var Wishlists = _context.Wishlists.Include(e => e.Product).Where(e => e.ApplicationUserId == userId);

            return Ok(Wishlists.ToList());
        }

        [HttpGet("{productId}")]
        public IActionResult AddToWishList(int productId)
        {
            //var user = await _userManager.GetUserAsync(User);

            //if (user == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var Wishlists = _context.Wishlists.Include(e => e.Product).Where(e => e.ApplicationUserId == userId);

            bool isFounded = false;

            foreach (var item in Wishlists)
            {
                if (item.ProductId == productId)
                {
                    isFounded = true;
                    break;
                }
            }

            if (!isFounded)
            {
                _context.Wishlists.Add(new()
                {
                    ProductId = productId,
                    ApplicationUserId = userId
                });

                _context.SaveChanges();
            }

            return Created();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var wishList = _context.Wishlists.FirstOrDefault(e => e.Id == id);

            if (wishList == null) return NotFound();

            _context.Wishlists.Remove(wishList);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
