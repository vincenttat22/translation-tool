using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }

        // GET: api/<UserListController>
        [HttpGet]
        [Route("List/{hide?}")]
        public ActionResult GetUserList(string hide)
        {
            var users = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Where(x => hide == null ? x.LockoutEnabled : (x.LockoutEnabled || !x.LockoutEnabled)).AsNoTracking().Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.UserName,
                x.LockoutEnabled,
                Roles = x.UserRoles.Select(u=>u.Role.Name)
            }).OrderByDescending(x=>x.LockoutEnabled).ToList();
            return Ok(users);
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UserApplicationUser(ApplicationUserModel user)
        {
            var _user = await _userManager.FindByIdAsync(user.Id);
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

        [HttpPatch]
        [Route("ActivateUser")]
        public async Task<IActionResult> ActivateApplicationUser(ApplicationUserModel user)
        {
            var _user = await _userManager.FindByIdAsync(user.Id);
            try
            {
                var result = await _userManager.SetLockoutEnabledAsync(_user, user.LockoutEnabled);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        [Route("CheckAvailableUserName")]
        public async Task<IActionResult> CheckAvailableUserName(CheckAvailableUserModel user)
        {
            ApplicationUser _user = new();
            if (user.UserName != null)
            {
                _user = await _userManager.FindByNameAsync(user.UserName);
            }
            else if (user.Email != null)
            {
                _user = _userManager.Users.Where(x => x.Email == user.Email && x.Id != user.Id).FirstOrDefault();
            }

            return Ok(new
            {
                FoundUser = _user != null
            });
        }

        // GET api/<UserListController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserListController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserListController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserListController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class ApplicationUserDto {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public IList<string> Roles { get; set; }
    }
}
