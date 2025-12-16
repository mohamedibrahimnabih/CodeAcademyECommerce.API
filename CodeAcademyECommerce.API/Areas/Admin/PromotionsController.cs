using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class PromotionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? code, int page = 1)
        {
            var promotions = _context.Promotions.AsQueryable();

            if(code is not null)
                promotions = promotions.Where(e=>e.Code.ToLower().Contains(code.ToLower().Trim()));

            double totalPages = Math.Ceiling(promotions.Count() / 5.0);
            int currentPage = page;
            promotions = promotions.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                promotions,
                totalPages,
                currentPage
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var promotion = _context.Promotions.FirstOrDefault(e => e.Id == id);

            if (promotion is null) return NotFound();

            return Ok(promotion);
        }

        [HttpPost]
        public IActionResult Create(PromotionCreateRequest promotionCreateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            Promotion promotion = new()
            {
                Code = promotionCreateRequest.Code,
                Discount = promotionCreateRequest.Discount,
                MaxOfUsage = promotionCreateRequest.MaxOfUsage,
                ProductId = promotionCreateRequest.ProductId,
                CreateById = userId
            };

            _context.Promotions.Add(promotion);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { promotion.Id }, new
            {
                success_msg = "Add Promotion Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, PromotionUpdateRequest promotionUpdateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var promotion = _context.Promotions.FirstOrDefault(e => e.Id == id);

            if (promotion is null) return NotFound();

            promotion.Code = promotionUpdateRequest.Code;
            promotion.Discount = promotionUpdateRequest.Discount;
            promotion.MaxOfUsage = promotionUpdateRequest.MaxOfUsage;
            promotion.UpdatedAT = DateTime.Now;
            promotion.UpdatedById = userId;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var promotion = _context.Promotions.FirstOrDefault(e => e.Id == id);

            if (promotion is null) return NotFound();

            _context.Promotions.Remove(promotion);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
