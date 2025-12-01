using Microsoft.AspNetCore.Authorization;

namespace WorkShop.Web.Security
{
    // Requirement นี้จะรับค่า PermissionCode (เช่น "AD002", "SC001") เข้ามาด้วย
    public class ActivePermissionRequirement : IAuthorizationRequirement
    {
        public string RequiredPermissionCode { get; }

        public ActivePermissionRequirement(string permissionCode)
        {
            RequiredPermissionCode = permissionCode;
        }
    }
}