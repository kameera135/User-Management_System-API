using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class Role
{
    public long RoleId { get; set; }

    public string Role1 { get; set; } = null!;

    public long? PlatformId { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public virtual Platform? Platform { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
