using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeAcademyECommerce.API.Areas.Identity
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.IDENTITY_AREA)]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            ApplicationUser user = new()
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                UserName = registerRequest.Name,
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Send Email Confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("Confirm", "Accounts", new { area = "Identity", user.Id, token }, Request.Scheme);

            var emailTemplateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate.html");

            var file = await System.IO.File.ReadAllTextAsync(emailTemplateDirectory);

            file = file.Replace("{{name}}", user.Name);
            file = file.Replace("{{link}}", link);

            await _emailSender.SendEmailAsync(user.Email, "Please Confirm Your Account In Ecommerce Code Academy App", file);

            return Ok(new
            {
                success_msg = "Add Account Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Confirm(string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.EmailOrUserName) ??
                await _userManager.FindByNameAsync(loginRequest.EmailOrUserName);

            if(user is null) return NotFound();

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!result)
                return BadRequest(new
                {
                    key = "EmailOrUserName",
                    msg = "Invalid Email Or User Name"
                });

            await _signInManager.SignInAsync(user, loginRequest.RememberMe);

            await _userManager.AddToRoleAsync(user, SD.CUSTOMER);

            /* YOUR CODE HERE */

            var jwtToken = new JwtSecurityToken(/* YOUR CODE HERE */);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expireIn = "14 day",
                success_msg = "Welcome Back!!",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }
    }
}
