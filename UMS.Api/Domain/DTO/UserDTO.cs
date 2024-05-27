namespace UMS.Api.Domain.DTO
{
    public class UserDTO
    {
        public long UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public byte[] PasswordHash { get; set; } = null!;
        
        public byte[] PasswordSalt { get; set; } = null!;
    }

    public class GetUserDTO
    {
        public string UserName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }

    public class UserPostDTO
    {
        public string UserName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string Password { get; set; } = null!;

        public long CreatedBy { get; set; }
    }

    public class UserPutDTO
    {
        public long UserId { get; set; } 

        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }
    }

    public class UserPlatformsAndRolesDTO
    {
        public long? PlatformId { get; set; }

        public long? RoleId { get; set; }

        public string? PlatformName { get; set; }

        public string? RoleName { get; set; }
    }

    public class BulkRoleAssignDTO
    {
        public string UserName { get; set; } = null!;

        public long RoleId { get; set; }
    }

    public class BulkPostDTO
    {
        public List<UserPostDTO> Users { get; set; } = null!;

        public List<BulkRoleAssignDTO>? UserRoles { get; set; }
    }

    public class RegisterUserDTO
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
        
        //public byte[] PasswordHash { get; set; } = null!;

        //public byte[] PasswordSalt { get; set; } = null!;
    }

    public class RegisterUserPostDTO
    {
        public string UserName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public byte[] PasswordHash { get; set; } = null!;

        public byte[] PasswordSalt { get; set; } = null!;

        public long? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? FirstLogin { get; set; }
    }

    public class UserLoginDTO
    {
        //public long UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Password { get; set; }
    }

    public class UserRolesAndPermissionsDTO
    {
        public long RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public long PermissionId { get; set; }

        public string Permission { get; set; } = null!;
    }

    public class AssignRolesToUserDTO
    {
        public long UserId { get; set;}
        public List<long>? RoleIds { get; set; }
    }

    public class RefreshTokenDTO 
    {
        public string? Token { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

    }

    public class ForgotPasswordDTO
    {
        public string? Email { get; set; }

        public string? ClientURI { get; set; }
    }

    public class ResetPasswordDTO
    {
        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public string? Email { get; set; }

        public string? Token { get; set; }
    }

    public class ResetPasswordTokenDTO
    {
        public string? Token { get; set; }

    }
}
