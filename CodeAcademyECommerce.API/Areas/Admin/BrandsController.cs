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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var brand = _context.Brands.FirstOrDefault(e => e.Id == id);

            if (brand is null) return NotFound();

            return Ok(new
            {
                brand
            });
        }

        [HttpPost]
        public IActionResult Create(BrandCreateRequest brandCreateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            Brand brand = new()
            {
                Name = brandCreateRequest.Name,
                Status = brandCreateRequest.Status
            };

            if (brandCreateRequest.Logo is not null && brandCreateRequest.Logo.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(brandCreateRequest.Logo.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(brandCreateRequest.Logo.FileName)}";

                brand.logo = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    brandCreateRequest.Logo.CopyTo(stream);
                }
            }

            brand.CreateById = userId;

            _context.Brands.Add(brand);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { brand.Id }, new
            {
                success_msg = "Add Brand Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, BrandUpdateRequest brandUpdateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var brand = _context.Brands.FirstOrDefault(e => e.Id == id);

            if (brand is null) return NotFound();

            if (brandUpdateRequest.Logo is not null && brandUpdateRequest.Logo.Length > 0)
            {
                // Delete file from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", brand.logo);

                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(brandUpdateRequest.Logo.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(brandUpdateRequest.Logo.FileName)}";

                brand.logo = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    brandUpdateRequest.Logo.CopyTo(stream);
                }
            }

            brand.Name = brandUpdateRequest.Name;
            brand.Status = brandUpdateRequest.Status;
            brand.UpdatedAT = DateTime.Now;
            brand.UpdatedById = userId;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(e => e.Id == id);

            if (brand is null) return NotFound();

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", brand.logo);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            _context.Remove(brand);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
