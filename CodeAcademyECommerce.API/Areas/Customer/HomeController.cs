using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Customer
{
    [Route(template: "[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? productName, long? minPrice, long? maxPrice, int? categoryId, int? brandId, int page = 1)
        {
            var products = _context.Products.AsQueryable();

            if (productName is not null)
                products = products.Where(e => e.Name.ToLower().Contains(productName.ToLower().Trim()));

            if (minPrice is not null)
                products = products.Where(e => (e.Price - (e.Price * (e.Discount / 100))) > minPrice);

            if (maxPrice is not null)
                products = products.Where(e => (e.Price - (e.Price * (e.Discount / 100))) < maxPrice);

            if (categoryId is not null)
                products = products.Where(e => e.CategoryId == categoryId);

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

        [HttpPost]
        [Authorize]
        public IActionResult SendMessage(MessageRequest messageRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            _context.Messages.Add(new()
            {
                SenderId = userId,
                Text = messageRequest.Message
            });
            _context.SaveChanges();

            return Ok(new
            {
                suceess_msg = "Add Ticket Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }
    }
}
