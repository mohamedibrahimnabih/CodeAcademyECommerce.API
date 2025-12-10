using CodeAcademyECommerce.API.Requests;
using CodeAcademyECommerce.API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CodeAcademyECommerce.API.Areas.Identity
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.IDENTITY_AREA)]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            /////////////////////////////////

            return Ok(new
            {
                success_msg = "Add Account Successfully",
                date = DateTime.Now,
                traceId = Guid.NewGuid().ToString()
            });
        }

        //public IActionResult Confirm(string id, string token)
        //{
        //    return NotFound();
        //}

        //public IActionResult Login()
        //{
        //    return NotFound();
        //}
    }
}
