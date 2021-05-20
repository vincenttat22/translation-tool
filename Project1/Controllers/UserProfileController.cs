using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<AppSettings> _appSettings;
        private string userId = "";
        public UserProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
            userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "UserId")?.Value;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                roles = roles.Result
            });
        }

        [HttpGet]
        [Route("GetUserFolders")]
        [Authorize]
        public async Task<IActionResult> GetUserFolders()
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = _userManager.GetRolesAsync(user);
            List<UserFolderModel> userFolder = new();  
            if (roles.Result.Contains("Admin"))
            {
                var users = _userManager.Users.ToList();
                foreach(var u in users)
                {     
                    userFolder.Add(BuildUserFolder(u)); 
                }        
            } else
            {          
                userFolder.Add(BuildUserFolder(user));
            }
            return Ok(userFolder);
        }

        private static UserFolderModel BuildUserFolder(ApplicationUser user) {
            List<UserFolderModel> userSubFolder = new();
            userSubFolder.Add(new() {Id= user.Id+"-input", Name = "Uploads" });
            userSubFolder.Add(new() { Id = user.Id + "-output", Name = "Translated Files" });
            return new()
            {
                Id = user.Id,
                Name = user.UserName,
                Children = userSubFolder
            };
        }
    }
}
