using CodeAcademyECommerce.API.DataAccess;
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
    }
}
