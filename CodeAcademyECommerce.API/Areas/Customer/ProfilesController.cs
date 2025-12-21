using CodeAcademyECommerce.API.DTOs.Requests;
using CodeAcademyECommerce.API.DTOs.Responses;
using CodeAcademyECommerce.API.Utilities;
using Mapster;
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
    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfilesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return NotFound();

            //UserVM userUpdateRequest = new()
            //{
            //    Address = user.Address,
            //    Email = user.Email,
            //    FullName = user.Name,
            //    PhoneNumber = user.PhoneNumber,
            //    UserName = user.UserName
            //};

            TypeAdapterConfig<ApplicationUser, UserResponse>
                .NewConfig()
                .Map(d => d.FullName, s => s.Name);

            //TypeAdapterConfig typeAdapterConfig = new();
            //typeAdapterConfig.NewConfig<ApplicationUser, UserVM>()
            //    .Map("FullName", "Name");

            UserResponse userResponse = user.Adapt<UserResponse>(/*typeAdapterConfig*/);

            return Ok(userResponse);
        }

        [HttpPut("{id}/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(string id, UserUpdateRequest userUpdateRequest)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.Name = userUpdateRequest.FullName;
            user.Email = userUpdateRequest.Email;
            user.Address = userUpdateRequest.Address;
            user.PhoneNumber = userUpdateRequest.PhoneNumber;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpPut("{id}/UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string id, PasswordUpdateRequest passwordUpdateRequest)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, passwordUpdateRequest.currentPassword, passwordUpdateRequest.newPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction("Login", "Account", new { area = "Identity" }, new
            {
                success_msg = "Change Password Successfully"
            });
        }

        [HttpPut("{id}/UpdateImage")]
        public async Task<IActionResult> UpdateImage(string id, ProfileImgUpdateRequest profileImgUpdateRequest)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            //Upload Img in wwwroot
            if (profileImgUpdateRequest.Img is not null && profileImgUpdateRequest.Img.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(profileImgUpdateRequest.Img.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(profileImgUpdateRequest.Img.FileName)}";

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\users", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    profileImgUpdateRequest.Img.CopyTo(stream);
                }

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\users", user.Img);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                //Update Img Column in DB
                user.Img = fileName;
            }

            //Save Changes
            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}
