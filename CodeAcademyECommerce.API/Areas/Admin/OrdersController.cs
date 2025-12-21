using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(int? orderId, int page = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var orders = _context.Orders.AsQueryable();

            if (orderId is not null)
                orders = orders.Where(e => e.Id == orderId);

            double totalPages = Math.Ceiling(orders.Count() / 5.0);
            int currentPage = page;
            orders = orders.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                totalPages,
                currentPage,
                orders
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _context.Orders.Include(e => e.ApplicationUser).FirstOrDefault(e => e.Id == id);

            if (order is null) return NotFound();

            var orderItems = _context.OrderItems.Include(e => e.Product).Where(e => e.OrderId == id);

            return Ok(new 
            {
                order,
                orderItems
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, OrderUpdateRequest orderUpdateRequest)
        {
            var orderInDb = _context.Orders.Include(e => e.ApplicationUser).FirstOrDefault(e => e.Id == id);

            if (orderInDb is null) return NotFound();

            orderInDb.OrderStatus = orderUpdateRequest.orderStatus;
            orderInDb.TrackingNumber = orderUpdateRequest.trackingNumber;
            orderInDb.CarrierName = orderUpdateRequest.carrierName;
            orderInDb.ShippedDate = DateTime.UtcNow;

            _context.SaveChanges();

            return NoContent();
        }
    }
}
