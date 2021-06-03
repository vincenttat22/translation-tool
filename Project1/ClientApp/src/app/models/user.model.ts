export class RolePermissions {
    RoleName: string;
    Permissions: Permission[];
}
export class Permission
{
    claimType: string;
    claimValues: string[];
}