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
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? msg, int page = 1)
        {
            var msgs = _context.Messages.AsQueryable();

            if (msg is not null)
                msgs = msgs.Where(e => e.Text.ToLower().Contains(msg.ToLower().Trim()));

            double totalPages = Math.Ceiling(msgs.Count() / 5.0);
            int currentPage = page;
            msgs = msgs.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                msgs,
                totalPages,
                currentPage
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var msg = _context.Messages.FirstOrDefault(e=>e.Id == id);

            if(msg is null) return NotFound();

            return Ok(msg);
        }

        [HttpPatch("{id}/MarkAsCompleted")]
        public IActionResult MarkAsCompleted(int id)
        {
            var msg = _context.Messages.FirstOrDefault(e => e.Id == id);

            if (msg is null) return NotFound();

            msg.TicketStatus = TicketStatus.Completed;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/Canceled")]
        public IActionResult Canceled(int id)
        {
            var msg = _context.Messages.FirstOrDefault(e => e.Id == id);

            if (msg is null) return NotFound();

            msg.TicketStatus = TicketStatus.Canceled;
            _context.SaveChanges();

            return NoContent();
        }
    }
}
