using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class ActivityLog
{
    public long LogId { get; set; }

    public long UserId { get; set; }

    public long? PlatformId { get; set; }

    public long? RoleId { get; set; }

    public string? ActivityType { get; set; }

    public string? Description { get; set; }

    public string? Details { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual Platform? Platform { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User User { get; set; } = null!;
}
