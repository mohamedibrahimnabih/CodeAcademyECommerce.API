using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public TicketController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> GetAll(string? name, int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();


            var tickets = _context.Messages.Where(e => e.SenderId == user.Id && !e.IsDeleted);

            if (filterVM.Name is not null)
                tickets = tickets.Where(e => e.Text.ToLower().Contains(filterVM.Name.ToLower().Trim()));

            double totalPages = Math.Ceiling(tickets.Count() / 5.0);
            int currentPage = filterVM.Page;

            return View(new TicketWithRelatedVM()
            {
                Messages = tickets.ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages,
            });
        }

        public IActionResult Delete(int id)
        {

            var message = _context.Messages.FirstOrDefault(e => e.Id == id);

            if (message is null) return NotFound();

            message.IsDeleted = true;
            message.TicketStatus = TicketStatus.Canceled;
            _context.SaveChanges();

            TempData["success-notification"] = "Delete Message Successfully";

            return RedirectToAction("Index");

        }
    }
}
