namespace UMS.Api.Domain.DTO
{
    public class PermissionDTO
    {
        public long PermissionId { get; set; }
        public string Permission { get; set; } = null!;
        public long PlatformId { get; set; }
        public string? PlatformName { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

    }

    public class PermissionPostDTO
    {
        public string Permission { get; set; } = null!;
        public long PlatformId { get; set; }
    }

    public class PermissionPutDTO
    {
        public long PermissionId { get; set; }
        public string Permission { get; set; } = null!;
    }

    public class RolesAndPlatformsGetDTO
    {
        public string? Role { get; set; }
        public string? Platform { get; set; } 
    }

    public class PermissionsForPlatformRoleGetDTO
    {
        public long PlatformId { get; set; }

        public string? Platform { get; set; }

        public long PermissionId { get; set; }

        public string? Permission { get; set; }
    }

    public class GetPermissionsNotInRoleDTO
    {
        public long PermissionId { get; set;}
        public string? Permission { get; set; }

    }

    public class PostPermissionsToRoleDTO
    {
        public long RoleId { get; set;}

        public List<long> PermissionIds { get; set; } = null!;
    }
}
