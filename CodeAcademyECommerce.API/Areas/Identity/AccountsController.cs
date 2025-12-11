using CodeAcademyECommerce.API.DataAccess;
using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
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
            var link = Url.Action(nameof(Confirm), "Accounts", new { area = "Identity", user.Id, token }, Request.Scheme);

            var emailTemplateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate.html");

            var file = await System.IO.File.ReadAllTextAsync(emailTemplateDirectory);

            file = file.Replace("{{name}}", user.Name);
            file = file.Replace("{{link}}", link);

            await _emailSender.SendEmailAsync(user.Email, "Please Confirm Your Account In Ecommerce Code Academy App", file);

            return CreatedAtAction(nameof(Login), new
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

            if (user is null)
                return BadRequest(new
                {
                    key = "EmailOrUserName",
                    msg = "Invalid Email Or User Name OR Password"
                });

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!result)
                return BadRequest(new
                {
                    key = "EmailOrUserName",
                    msg = "Invalid Email Or User Name OR Password"
                });

            await _signInManager.SignInAsync(user, loginRequest.RememberMe);

            await _userManager.AddToRoleAsync(user, SD.CUSTOMER);

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, DateTime.Now.ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lp4ywiPp4zHCe1tmAmIGXoF7829twxfN"));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: "https://localhost:7270",
                audience: "https://localhost:4200,https://localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddDays(14),
                signingCredentials: signingCredentials
            );


            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expireIn = "14 day",
                success_msg = "Welcome Back!!",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPost]
        [Route("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest resendEmailConfirmationRequest)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationRequest.EmailOrUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationRequest.EmailOrUserName);

            if (user is null) return NotFound();


            // Send Email Confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(Confirm), "Accounts", new { area = "Identity", user.Id, token }, Request.Scheme);

            var emailTemplateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate.html");

            var file = await System.IO.File.ReadAllTextAsync(emailTemplateDirectory);

            file = file.Replace("{{name}}", user.Name);
            file = file.Replace("{{link}}", link);

            await _emailSender.SendEmailAsync(user.Email!, "Resend - Please Confirm Your Account In Ecommerce Code Academy App", file);

            return Ok(new
            {
                success_msg = "Send Email Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordRequest.EmailOrUserName) ?? await _userManager.FindByNameAsync(forgetPasswordRequest.EmailOrUserName);

            if (user is null) return NotFound();

            var otp = new Random().Next(1000, 9999);

            var count = _context.ApplicationUserOTPs.Count(e => e.ApplicationUserId == user.Id && e.CreatedAT >= DateTime.Now.AddHours(-24));

            if (count >= 4)
            {
                return BadRequest(new
                {
                    error_msg = "Too Many Atempts, Please Try Again Later",
                    date = DateTime.Now,
                    traceId = Guid.NewGuid().ToString()
                });
            }

            else
            {
                _context.ApplicationUserOTPs.Add(new ApplicationUserOTP()
                {
                    OTP = otp.ToString(),
                    ApplicationUserId = user.Id,
                });
                _context.SaveChanges();

                await _emailSender.SendEmailAsync(user.Email, "Please Password Reset Your Account In Ecommerce Code Academy App",
                    $"<h1>Password Reset By Using OTP {otp}</h1>");

                return CreatedAtAction(nameof(ValidateOTP), new
                {
                    id = user.Id,
                }, new
                {
                    success_msg = "Send Email Successfully",
                    date = DateTime.Now,
                    traceId = Guid.NewGuid().ToString()
                });
            }
        }

        [HttpPost]
        [Route("ValidateOTP")]
        public IActionResult ValidateOTP(ValidateOTPRequest validateOTPRequest)
        {
            var otp = _context.ApplicationUserOTPs.OrderBy(e => e.Id).LastOrDefault(e => e.ApplicationUserId == validateOTPRequest.Id && !e.IsUsed && e.ValidTo > DateTime.UtcNow);

            if (otp.OTP == validateOTPRequest.OTP)
            {
                otp.IsUsed = true;
                _context.SaveChanges();

                return CreatedAtAction(nameof(NewPassword), new { id = validateOTPRequest.Id }, new
                {
                    success_msg = "Valid OTP",
                    date = DateTime.Now,
                    traceId = Guid.NewGuid().ToString()
                });

            }

            return BadRequest(new
            {
                error_msg = "Invalid OTP",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        [HttpPost]
        [Route("NewPassword")]
        public async Task<IActionResult> NewPassword(NewPasswordRequest newPasswordRequest)
        {

            var user = await _userManager.FindByIdAsync(newPasswordRequest.Id);

            if (user is null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordRequest.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(Login), new
            {
                success_msg = "Change Password Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }
    }
}
