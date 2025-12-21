using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeAcademyECommerce.API.Areas.Customer
{
    [Route(template: "[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TicketsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? name, int page = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            //var user = await _userManager.GetUserAsync(User);

            //if (user is null) return NotFound();

            var tickets = _context.Messages.Where(e => e.SenderId == userId && !e.IsDeleted);

            if (name is not null)
                tickets = tickets.Where(e => e.Text.ToLower().Contains(name.ToLower().Trim()));

            double totalPages = Math.Ceiling(tickets.Count() / 5.0);
            int currentPage = page;

            return Ok(new 
            {
                tickets,
                currentPage,
                totalPages,
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            var message = _context.Messages.FirstOrDefault(e => e.Id == id);

            if (message is null) return NotFound();

            message.IsDeleted = true;
            message.TicketStatus = TicketStatus.Canceled;
            _context.SaveChanges();

            return NoContent();
        }
    }
}
