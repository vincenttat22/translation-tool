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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<AppSettings> _appSettings;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings)
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
                var role = new ApplicationRole()
                {
                    Name = "Admin"
                };
                var applicationUser = new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "Default"
                };
                try
                {
                    var roleRS = await _roleManager.CreateAsync(role);
                    var claimRS = await _roleManager.AddClaimAsync(role, new Claim("AllAccess", "all_access"));
                    var userRS = await _userManager.CreateAsync(applicationUser, "Rea!!yStr0ng");
                    if(userRS.Succeeded && roleRS.Succeeded && claimRS.Succeeded)
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
        [Route("Login")]
        public async Task<IActionResult> PostApplicationUserLogin(LoginModel loginModel)
        {
            var user = loginModel.UserName.Contains("@") ? await _userManager.FindByEmailAsync(loginModel.UserName) : await _userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && ! await _userManager.GetLockoutEnabledAsync(user))
            {
                return BadRequest(new { message = "Account is deactivated! Please, contact Admin." });
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password,false,true);
            if (result.Succeeded)
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
            }

            if(result.IsLockedOut)
            {
                return BadRequest(new { message = "Account is locked! Due to mutiple false authentication attempts." });
            } else
            {
                return BadRequest(new { message = "Username or password is incorrect." });
            }
            //if (user != null && !await _userManager.GetLockoutEnabledAsync(user))
            //{
            //    return BadRequest(new { message = "Account is deactivated. Please, contact Admin." });
            //}
            //if (user != null && await _userManager.CheckPasswordAsync(user,loginModel.Password))
            //{
            //    var tokenDescriptor = new SecurityTokenDescriptor
            //    {
            //        Subject = new ClaimsIdentity(new Claim[]
            //        {
            //            new Claim("UserId", user.Id.ToString())
            //        }),
            //        Expires = DateTime.UtcNow.AddDays(5),
            //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.Jwt_key)), SecurityAlgorithms.HmacSha256Signature)
            //    };
            //    var tokenHanlder = new JwtSecurityTokenHandler();
            //    var sercurityToken = tokenHanlder.CreateToken(tokenDescriptor);
            //    var token = tokenHanlder.WriteToken(sercurityToken);
            //    return Ok(new { token });
            //} else
            //{
            //    return BadRequest(new { message = "Username or password is incorrect." });
            //}
        }
    }
}
