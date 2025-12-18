using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Customer
{
    [Route(template: "[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER_AREA)]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Product? product = _context.Products.Include(e => e.CreateBy).FirstOrDefault(e => e.Id == id);

            if (product is null || product.CreateBy is null)
                return NotFound();

            product.Traffic += 1;
            _context.SaveChanges();

            var productInSameCategory = _context.Products.Include(e => e.Category).Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id).Skip(0).Take(4);

            var productInSameBrand = _context.Products.Include(e => e.Category).Where(e => e.BrandId == product.BrandId && e.Id != product.Id).Skip(0).Take(4);

            var productsInSameName = _context.Products.Include(e => e.Category).Where(e => e.Name.ToLower().Contains(product.Name) && e.Id != product.Id).Skip(0).Take(4);

            var minPrice = product.Price - (product.Price * 0.1m);
            var maxPrice = product.Price + (product.Price * 0.1m);
            var productInSamePrice = _context.Products.Include(e => e.Category).Where(e => e.Price > minPrice && e.Price < maxPrice && e.Id != product.Id);

            var topProudcts = _context.Products.Include(e => e.Category).Where(e => e.Id != product.Id).OrderByDescending(e => e.Traffic).Skip(0).Take(4);

            var ratings = _context.Ratings.Include(e => e.ApplicationUser).Where(e => e.ProductId == id);
            var ratingReplys = _context.RatingReplies.Include(e => e.ApplicationUser)
                .Where(e => ratings.Select(e => e.Id).Contains(e.RatingId));

            return Ok(new
            {
                product,
                productInSameCategory,
                productsInSameName,
                productInSamePrice,
                topProudcts,
                productInSameBrand,
                ratings,
                ratingReplys,
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Reply(ReplyRequest replyRequest)
        {
            //var user = await _userManager.GetUserAsync(User);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var product = _context.Products.FirstOrDefault(e => e.CreateById == userId && e.Id == replyRequest.ProductId);

            var ratingReply = _context.RatingReplies.FirstOrDefault(e => e.RatingId == replyRequest.RatingId);

            if (product is null) return BadRequest(new
            {
                key = "InvalidOperation",
                msg = "You can't reply to this product, you don't have the ownership"
            });
            else if (ratingReply is null) return NotFound(new
            {
                key = "InvalidOperation",
                msg = "Rating is Not Found"
            });
            else
            {
                _context.RatingReplies.Add(new()
                {
                    ApplicationUserId = userId,
                    Comment = replyRequest.Reply,
                    RatingId = replyRequest.RatingId
                });

                _context.SaveChanges();

                return Created($"{Request.Scheme}://{Request.Host}/Customer/Details/{replyRequest.ProductId}#comments", new
                {
                    suceess_msg = "Add Reply Successfully",
                    date = DateTime.Now,
                    traceId = Guid.NewGuid().ToString()
                });
            }
        }

        [HttpPatch("{productId}/IncrementRate")]
        [Authorize]
        public IActionResult IncrementRate(int productId, int commentId)
        {
            var rate = _context.Ratings.FirstOrDefault(e => e.Id == commentId);

            if (rate is null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var isFounded = _context.UserRatings.Any(e => e.ApplicationUserId == userId && e.RatingId == commentId);

            if (!isFounded)
            {
                rate.Rank += 1;

                _context.UserRatings.Add(new()
                {
                    RatingId = commentId,
                    ApplicationUserId = userId
                });

                _context.SaveChanges();
            }

            return Ok(new
            {
                isFounded
            });

        }

        [HttpPatch("{productId}/DecrementRate")]
        [Authorize]
        public IActionResult DecrementRate(int productId, int commentId)
        {
            var rate = _context.Ratings.FirstOrDefault(e => e.Id == commentId);

            if (rate is null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var isFounded = _context.UserRatings.Any(e => e.ApplicationUserId == userId && e.RatingId == commentId);

            if (!isFounded)
            {
                rate.Rank -= 1;

                _context.UserRatings.Add(new()
                {
                    RatingId = commentId,
                    ApplicationUserId = userId
                });

                _context.SaveChanges();
            }

            return Ok(new
            {
                isFounded
            });
        }

        
    }
}
