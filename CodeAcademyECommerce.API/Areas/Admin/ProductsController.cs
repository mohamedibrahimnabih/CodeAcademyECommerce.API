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
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? productName, long? minPrice, long? maxPrice, int? categoryId, int? brandId, int page = 1)
        {
            var products = _context.Products.AsQueryable();

            if (productName is not null)
                products = products.Where(e=>e.Name.ToLower().Contains(productName.ToLower().Trim()));

            if (minPrice is not null)
                products = products.Where(e => (e.Price - (e.Price * (e.Discount / 100))) > minPrice);

            if (maxPrice is not null)
                products = products.Where(e => (e.Price - (e.Price * (e.Discount / 100))) < maxPrice);

            if (categoryId is not null)
                products = products.Where(e=>e.CategoryId == categoryId);

            if (brandId is not null)
                products = products.Where(e => e.BrandId == brandId);

            double totalPages = Math.Ceiling(products.Count() / 5.0);
            int currentPage = page;
            products = products.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                products,
                totalPages,
                currentPage
            });
        }

        //[HttpGet("{id}")]
        //public IActionResult GetById(int id);

        //[HttpPost]
        //public IActionResult Create(Body);

        //[HttpPut("{id}")]
        //public IActionResult Update(int id, Body);

        //[HttpDelete("{id}")]
        //public IActionResult Delete(int id);

    }
}
