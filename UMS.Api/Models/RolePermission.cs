using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class RolePermission
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long PermissionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
