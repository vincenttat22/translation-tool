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
    public class UserListController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserListController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }

        // GET: api/<UserListController>
        [HttpGet]
        public ActionResult GetUserList()
        {
            var users = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsNoTracking().Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.UserName,
                x.LockoutEnabled,
                Roles = x.UserRoles.Select(u=>u.Role.Name)
            }).ToList();
            return Ok(users);
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
