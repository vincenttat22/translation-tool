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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _applicationContext;
        private readonly IOptions<AppSettings> _appSettings;
        private string userId = "";
        public UserProfileController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationContext applicationContext, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _applicationContext = applicationContext;
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
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                roles = roles.Result
            });
        }

        [HttpGet]
        [Route("GetUserRoles")]
        [Authorize]
        public ActionResult GetUserRole()
        {
            var roles = _roleManager.Roles.Select(x => x.Name).ToList();      
            return Ok(roles);
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

        private UserFolderModel BuildUserFolder(ApplicationUser user) {
            List<UserFolderModel> userSubFolder = new();
            userSubFolder.Add(new() { Id = user.Id+"/input", Name = "Uploads", Files = BuildUserFiles(user,"input") });
            userSubFolder.Add(new() { Id = user.Id + "/output", Name = "Translated Files", Files = BuildUserFiles(user, "output") });
            return new()
            {
                Id = user.Id,
                Name = user.UserName,
                Children = userSubFolder
            };
        }

        private List<UserFileModel> BuildUserFiles(ApplicationUser user, string fileType)
        {
            return _applicationContext.FileManagement.Where(x => x.UserId == user.Id && x.FileType == fileType).Select(x =>
               new UserFileModel
               {
                   Id = x.Id,
                   OriginalFileName = x.OriginalFileName,
                   LanguageCode = x.LanguageCode,
                   LastUpdated = x.LastUpdated
               }).ToList();
        }
    }
}
