using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<AppSettings> _appSettings;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        [HttpGet]
        [Route("Initiate")]
        public async Task<IActionResult> InitiateApplicationUser()
        {
            var allUser =  _userManager.Users.ToList();
            if (allUser.FirstOrDefault() == null)
            {
                var role = new IdentityRole()
                {
                    Name = "Admin"
                };
                var applicationUser = new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "admin",
                    FirstName = "Admin",
                    LastName = "Default"
                };
                try
                {
                    var roleRS = await _roleManager.CreateAsync(role);
                    var userRS = await _userManager.CreateAsync(applicationUser, "Rea!!yStr0ng");
                    if(roleRS.Succeeded && roleRS.Succeeded)
                    {
                        var rs = await _userManager.AddToRoleAsync(applicationUser, "Admin");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Ok("true");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> PostApplicationUser(ApplicationUserModel user)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            try
            {
                var result = await _userManager.CreateAsync(applicationUser, user.Password);
                return Ok(result);
            } catch (Exception)
            {       
                throw ;
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> PostApplicationUserLogin(LoginModel loginModel)
        {
            var user = loginModel.UserName.Contains("@") ? await _userManager.FindByEmailAsync(loginModel.UserName) : await _userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user,loginModel.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserId", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.Jwt_key)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHanlder = new JwtSecurityTokenHandler();
                var sercurityToken = tokenHanlder.CreateToken(tokenDescriptor);
                var token = tokenHanlder.WriteToken(sercurityToken);
                return Ok(new { token });
            } else
            {
                return BadRequest(new { message = "Username or password is incorrect." });
            }
        }
    }
}
