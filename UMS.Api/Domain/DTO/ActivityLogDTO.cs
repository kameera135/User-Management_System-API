namespace UMS.Api.Domain.DTO
{
    public class ActivityLogDTO
    {
        public long LogId { get; set; }

        public string UserName { get; set; } = null!;

        public long UserId { get; set; }

        public string? PlatformName { get; set; }

        public long? PlatformId { get; set; }

        public string? RoleName { get; set; }

        public string? ActivityType { get; set; }

        public string? Description { get; set; }

        public string? Details { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class ActivityLogPostDTO
    {
        public long UserId { get; set; }

        public long? PlatformId { get; set; }

        public long? RoleId { get; set; }

        public string? ActivityType { get; set; }

        public string? Description { get; set; }

        public string? Details { get; set; }
    }
}
