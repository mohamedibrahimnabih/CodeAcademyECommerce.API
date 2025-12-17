using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeAcademyECommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult GetAll(string? name, int page = 1)
        {

            var users = _userManager.Users.AsQueryable();

            if (name is not null)
                users = users.Where(e => e.Name.ToLower().Contains(name.ToLower().Trim()));

            double totalPages = Math.Ceiling(users.Count() / 5.0);
            int currentPage = page;
            users = users.Skip((page - 1) * 5).Take(5);

            return Ok(new
            {
                users,
                currentPage,
                totalPages
            });
        }

        [HttpGet("Roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.AsQueryable();

            return Ok(roles);
        }

        [HttpPost]
        public IActionResult Create(UserCreateRequest userCreateRequest)
        {
            /* YOUR CODE HERE */

            return CreatedAtAction(nameof(GetAll), new
            {
                success_msg = "Add Account Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }
    }
}
