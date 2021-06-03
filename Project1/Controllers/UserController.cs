using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpPost]
        [Route("AddEditRole")]
        public async Task<IActionResult> PostAddRole(ApplicationRoleModel data)
        {
            var role = new ApplicationRole()
            {
                Name = data.Name
            };
            try
            {
                var roleRS = await _roleManager.CreateAsync(role);
                if(roleRS.Succeeded)
                {
                    var claimRS = await _roleManager.AddClaimAsync(role, new Claim("Home", "show_translation"));
                    return Ok(claimRS);
                }
                return Ok(roleRS);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut]
        [Route("AddEditRole")]
        public async Task<IActionResult> PutEditRole(ApplicationRoleModel data)
        {
            try
            {
                var thisRole = await _roleManager.FindByNameAsync(data.Name);
                thisRole.Name = data.NewName;
                var roleRS = await _roleManager.UpdateAsync(thisRole);
                return Ok(roleRS);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpDelete]
        [Route("AddEditRole/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            try
            {
                var thisRole = await _roleManager.FindByNameAsync(roleName);
                var roleRS = await _roleManager.DeleteAsync(thisRole);
                return Ok(roleRS);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        [Route("GetRolePermissions/{roleName?}")]
        public async Task<IActionResult> GetPermissions(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var claims = await _roleManager.GetClaimsAsync(role);
            var result = claims.Select(x => x.Value).ToArray();
            List<Permission> permissions = new();
            if (claims.Count <= 0)
            {
                return NotFound();
            }

            var checkAllAccess = claims.FirstOrDefault();
            if (checkAllAccess.Type == "AllAccess")
            {
                var _roleClaimModel = new ApplicationRoleClaimModel();
                permissions = _roleClaimModel.DefaultPermissions;
            }
            else
            {
                var claimTypes = claims.Select(x => x.Type).Distinct();
                foreach(var claimType in claimTypes)
                {
                    var claimValues = claims.Where(x => x.Type == claimType).Select(x => x.Value).ToArray();
                    permissions.Add(new Permission()
                    {
                        ClaimType = claimType,
                        ClaimValues = claimValues
                    });
                }

            }
            return Ok(permissions);
        }

        [HttpPatch]
        [Route("UpdateRolePermissions")]
        public async Task<IActionResult> UpdateRolePermissions(PermissionParams permissionParams)
        {
            var role = await _roleManager.FindByNameAsync(permissionParams.RoleName);
            var _claims = await _roleManager.GetClaimsAsync(role);
            var _arrClaims = _claims.Select(x => x.Value).ToArray();
            var toDelete = _arrClaims.Except(permissionParams.ClaimValues);
            var toAdd = permissionParams.ClaimValues.Except(_arrClaims);
            try
            {
                foreach (var a in toAdd)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(permissionParams.ClaimType, a));
                }
                foreach (var d in toDelete)
                {
                    var _claim = _claims.FirstOrDefault(x => x.Value == d);
                    await _roleManager.RemoveClaimAsync(role, _claim);
                }
                return Ok(IdentityResult.Success);
            }catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
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
    }

    public class ApplicationUserDto {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public IList<string> Roles { get; set; }
    }
}
