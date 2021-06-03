using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class ApplicationUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }

        public bool LockoutEnabled { get; set; }
    }

    public class ApplicationRoleModel
    {
        public string Name { get; set; }
        public string NewName { get; set; }
    }

    public class ApplicationRoleClaimModel
    {
        public ApplicationRoleClaimModel()
        {
            DefaultPermissions = new()
            {
                new Permission()
                {
                    ClaimType = "Home",
                    ClaimValues = new string[]
                    {
                        "show_translation",
                        "show_userlist_role",
                        "show_userwork_outline"
                    }
                },
                new Permission()
                {
                    ClaimType = "User",
                    ClaimValues = new string[]
                    {
                        "show_userlist",
                        "show_userroles",
                        "create",
                        "edit",
                        "deactivate",
                        "create_userrole",
                        "edit_userrole",
                        "delete_userrole"
                    }
                }
            };
        }
        public string RoleName { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<Permission> DefaultPermissions { get; set; }
    }

    public class Permission
    {
        public string ClaimType { get; set; }
        public string[] ClaimValues { get; set; }
    }

    public class PermissionParams
    {
        public string RoleName { get; set; }
        public string ClaimType { get; set; }
        public string[] ClaimValues { get; set; }
    }
}
