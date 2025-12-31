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
    //[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.FirstOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create(ProductCreateRequest productCreateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            Product product = new()
            {
                Name = productCreateRequest.Name,
                Description = productCreateRequest.Description,
                BrandId = productCreateRequest.BrandId,
                CategoryId = productCreateRequest.CategoryId,
                Status = productCreateRequest.Status,
                Price = productCreateRequest.Price,
                Discount = productCreateRequest.Discount,
                Quantity = productCreateRequest.Quantity,
                CreateById = userId,
            };

            if (productCreateRequest.MainImg is not null && productCreateRequest.MainImg.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(productCreateRequest.MainImg.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(productCreateRequest.MainImg.FileName)}";

                product.MainImg = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    productCreateRequest.MainImg.CopyTo(stream);
                }
            }

            _context.Add(product);
            _context.SaveChanges();

            if(productCreateRequest.SubImages is not null && productCreateRequest.SubImages.Count > 0)
            {
                foreach (var item in productCreateRequest.SubImages)
                {
                    // Save file in wwwroot
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.FileName);

                    string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(item.FileName)}";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs\\sub_imgs", fileName);

                    //if (System.IO.File.Exists(filePath))
                    //    System.IO.File.Create(filePath);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    // Save file name in DB
                    _context.ProductSubImgs.Add(new()
                    {
                        SubImg = fileName,
                        ProductId = product.Id
                    });
                }

                _context.SaveChanges();
            }

            if(productCreateRequest.Colors is not null && productCreateRequest.Colors.Count > 0)
            {
                foreach (var item in productCreateRequest.Colors)
                {
                    _context.ProductColors.Add(new()
                    {
                        Color = item,
                        ProductId = product.Id
                    });
                }

                _context.SaveChanges();
            }

            return CreatedAtAction(nameof(GetById), new { product.Id }, new
            {
                success_msg = "Add Product Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductUpdateRequest productUpdateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null) return Unauthorized();

            var product = _context.Products.FirstOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            product.Name = productUpdateRequest.Name;
            product.Description = productUpdateRequest.Description;
            product.Price = productUpdateRequest.Price;
            product.Discount = productUpdateRequest.Discount;
            product.Quantity = productUpdateRequest.Quantity;
            product.BrandId = productUpdateRequest.BrandId;
            product.CategoryId = productUpdateRequest.CategoryId;
            product.UpdatedById = userId;
            product.UpdatedAT = DateTime.Now;

            if (productUpdateRequest.MainImg is not null && productUpdateRequest.MainImg.Length > 0)
            {
                // Remove old img from wwwroot
                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs", product.MainImg);

                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(productUpdateRequest.MainImg.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(productUpdateRequest.MainImg.FileName)}";

                product.MainImg = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    productUpdateRequest.MainImg.CopyTo(stream);
                }
            }

            if (productUpdateRequest.SubImages is not null && productUpdateRequest.SubImages.Count > 0)
            {
                var productSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == id);

                foreach(var item in productSubImgs)
                {
                    // Remove old img from wwwroot
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs\\sub_imgs", item.SubImg);

                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                _context.ProductSubImgs.RemoveRange(productSubImgs);

                foreach (var item in productUpdateRequest.SubImages)
                {
                    // Save file in wwwroot
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.FileName);

                    string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(item.FileName)}";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs\\sub_imgs", fileName);

                    //if (System.IO.File.Exists(filePath))
                    //    System.IO.File.Create(filePath);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    // Save file name in DB
                    _context.ProductSubImgs.Add(new()
                    {
                        SubImg = fileName,
                        ProductId = product.Id
                    });
                }
            }

            if (productUpdateRequest.Colors is not null && productUpdateRequest.Colors.Count > 0)
            {
                var productColors = _context.ProductColors.Where(e => e.ProductId == id);

                _context.ProductColors.RemoveRange(productColors);

                foreach (var item in productUpdateRequest.Colors)
                {
                    _context.ProductColors.Add(new()
                    {
                        Color = item,
                        ProductId = product.Id
                    });
                }
            }

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}/DeleteSubImg")]
        public IActionResult DeleteSubImg(int id)
        {
            var productSubImg = _context.ProductSubImgs.FirstOrDefault(e => e.Id == id);

            if (productSubImg is null) return NotFound();

            // Remove old img from wwwroot
            string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs\\sub_imgs", productSubImg.SubImg);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            _context.ProductSubImgs.Remove(productSubImg);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}/DeleteColor")]
        public IActionResult DeleteColor(int id)
        {
            var productColor = _context.ProductColors.FirstOrDefault(e => e.Id == id);

            if (productColor is null) return NotFound();

            _context.ProductColors.Remove(productColor);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            string oldMainImgFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs", product.MainImg);

            if (System.IO.File.Exists(oldMainImgFilePath))
                System.IO.File.Delete(oldMainImgFilePath);

            var productSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == id);

            foreach (var item in productSubImgs)
            {
                // Remove old img from wwwroot
                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_imgs\\sub_imgs", item.SubImg);

                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
