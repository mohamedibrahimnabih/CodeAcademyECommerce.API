using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeAcademyECommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class BrandsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? brandName, int page = 1)
        {
            var brands = _context.Brands.AsQueryable();

            if(brandName is not null)
                brands = brands.Where(e => e.Name.ToLower().Contains(brandName.Trim().ToLower()));

            int currentPage = page;
            double totalPages = Math.Ceiling(brands.Count() / 5.0);
            brands = brands.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                brands,
                currentPage,
                totalPages
            });
        }
    }
}
