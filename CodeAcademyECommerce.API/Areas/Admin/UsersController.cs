using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        private readonly IEmailSender _emailSender;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);

            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Name,
                user.Email,
                roles
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateRequest userCreateRequest)
        {
            ApplicationUser user = new()
            {
                Name = userCreateRequest.Name,
                Email = userCreateRequest.Email,
                PhoneNumber = userCreateRequest.PhoneNumber,
                UserName = userCreateRequest.UserName,
                EmailConfirmed = userCreateRequest.EmailConfirmation
            };

            var result = await _userManager.CreateAsync(user, userCreateRequest.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (!userCreateRequest.EmailConfirmation)
            {
                // Send Email Confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action("Confirm", "Account", new { area = "Identity", user.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Pleas Confirm Your Account In Ecommerce Code Academy App",
                    $"<h1>Please Confirm You Account By clicking <a href='{link}'>Here</a></h1>");
            }

            await _userManager.AddToRoleAsync(user, userCreateRequest.Role);

            return CreatedAtAction(nameof(GetAll), new
            {
                success_msg = "Add Account Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, string role)
        {
            return NoContent();
        }
    }
}
