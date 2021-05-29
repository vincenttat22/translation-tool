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
                    var userRS = await _userManager.CreateAsync(applicationUser, "Rea!!yStr0ng");
                    if(userRS.Succeeded && roleRS.Succeeded)
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
                await _userManager.AddToRolesAsync(applicationUser, user.Roles);
                return Ok(result);
            } catch (Exception)
            {       
                throw ;
            }
        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UserApplicationUser(ApplicationUserModel user)
        {
            var _user = await _userManager.FindByIdAsync(user.Id);
            await _userManager.SetLockoutEnabledAsync(_user, false);
            var _userRoles = _userManager.GetRolesAsync(_user).Result;
            var diff = user.Roles.Except(_userRoles).ToArray();
            var diff2 = _userRoles.Except(user.Roles).ToArray();
            Console.WriteLine(diff);
            Console.WriteLine(diff2);
            if (user.Password != null)
            {
                _user.PasswordHash = _userManager.PasswordHasher.HashPassword(_user, user.Password);
            }
            _user.Email = user.Email;
            _user.FirstName = user.FirstName;
            _user.LastName = user.LastName;
            try
            {
                var result = await _userManager.UpdateAsync(_user);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("CheckAvailableUserName")]
        public async Task<IActionResult> CheckAvailableUserName(CheckAvailableUserModel user)
        {
            ApplicationUser _user = new();
            if(user.UserName != null)
            {
                _user = await _userManager.FindByNameAsync(user.UserName);
            } else if (user.Email != null)
            {
                _user = _userManager.Users.Where(x => x.Email == user.Email && x.Id != user.Id).FirstOrDefault();
            }

            return Ok(new
            {
                FoundUser = _user != null
            });
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
