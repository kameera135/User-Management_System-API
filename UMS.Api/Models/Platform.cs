using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class Platform
{
    public long PlatformId { get; set; }

    public string PlatformName { get; set; } = null!;

    public string? PlatformCode { get; set; }

    public string? PlatformUrl { get; set; }

    public string? Description { get; set; }

    public bool? ExternalLink { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<UserPlatform> UserPlatforms { get; set; } = new List<UserPlatform>();
}