namespace UMS.Api.Domain.DTO
{
    public class RoleDTO
    {
        public long RoleId { get; set; }
        public string Role { get; set; } = null!;
        public long? platformId { get; set; }
        public string platform { get; set; } = null!;
        public bool? status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class RolePostDTO
    {
        public string Role { get; set; } = null!;
        public List<long> PlatformIds { get; set; } = null!;
    }

    public class RolePostBulkDTO
    {
        public string Role { get; set; } = null!;
        public List<long>? PlatformId { get; set;}
    }

    public class RolePutDTO
    {
        public long RoleId { get; set; }
        public string Role { get; set; } = null!;
        public List<long> PlatformIds { get; set; } = null!;
        public bool? Status { get; set; }

    }

    public class GetPermissionDTO
    {
        public long platformId { get; set; }
        public string Platform { get; set; } = null !;
        public long permissionId { get; set; }
        public string? Permission { get; set; }
    }

    public class GetRolesForPlatformUsersDTO
    {
        public long RoleId { get; set; }
        public string? Role { get; set; }
        public long? PlatformId { get; set;}
        public string? PlatformName { get; set; }

    }

    public class GetRolesNotForUserDTO
    {
        public long RoleId { get; set; }

        public string Role { get; set; } = null!;
    }

}
