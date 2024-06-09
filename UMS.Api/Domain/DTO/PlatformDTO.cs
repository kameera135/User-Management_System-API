namespace UMS.Api.Domain.DTO
{
    public class PlatformDTO
    {
        public long PlatformId { get; set; }

        public string PlatformName { get; set; } = null!;

        public string? PlatformCode { get; set; }

        public string? Description { get; set; }

        public string? PlatformUrl { get; set; }

        public bool? ExternalLink { get; set; }
    }

    public class PlatformPostDTO
    {
        public string? PlatformCode { get; set; }

        public string PlatformName { get; set; } = null!;

        public string? PlatformUrl { get; set; }

        public string? Description { get; set; }

        public bool? ExternalLink { get; set; }
    }

    public class PlatformPutDTO
    {
        public long PlatformId { get; set; }

        public string? PlatformCode { get; set; }

        public string PlatformName { get; set; } = null!;

        public string? PlatformUrl { get; set; }

        public string? Description { get; set; }

        public bool? ExternalLink { get; set; }
    }

    public class ComboboxDTO
    {
        public long Id { get; set; }

        public string Value { get; set; } = null!;
    }

    public class PlatformsOfRoleDTO
    {
        public long PlatformId { get; set; }
        public string PlatformName { get; set; } = null!;
    }

    public class PlatformUsersDTO
    {
        public long UserId { get; set; }

        public string? UserName { get; set; }

        public long PlatformId { get; set; }

        public string? PlatformName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }
    }

    public class AssignPlatformUsersDTO
    {
        public long PlatformId { get; set; }
        public List<long> UserIds { get; set; } = null!;
    }
}