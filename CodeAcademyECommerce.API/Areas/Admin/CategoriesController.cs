using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? categoryName, int page = 1)
        {
            var categories = _context.Categories.AsQueryable();

            if(categoryName is not null)
                categories = categories.Where(e => e.Name.ToLower().Contains(categoryName.Trim().ToLower()));

            int currentPage = page;
            double totalPages = Math.Ceiling(categories.Count() / 5.0);
            categories = categories.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                categories,
                totalPages,
                currentPage
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(e => e.Id == id);

            if(category is null) return NotFound();

            return Ok(new
            {
                category
            });
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateRequest categoryCreateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            Category category = new()
            {
                Name = categoryCreateRequest.Name,
                Status = categoryCreateRequest.Status
            };

            category.CreateById = userId;

            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { category.Id }, new
            {
                success_msg = "Add Category Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryUpdateRequest categoryUpdateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var category = _context.Categories.FirstOrDefault(e => e.Id == id);

            if (category is null) return NotFound();

            category.Name = categoryUpdateRequest.Name;
            category.Status = categoryUpdateRequest.Status;
            category.UpdatedAT = DateTime.Now;
            category.UpdatedById = userId;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(e => e.Id == id);

            if (category is null) return NotFound();

            _context.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
