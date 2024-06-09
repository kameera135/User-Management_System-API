using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class Permission
{
    public long PermissionId { get; set; }

    public string Permission1 { get; set; } = null!;

    public long PlatformId { get; set; }

    public bool? Status { get; set; }

    public bool? IsLicence { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual Platform Platform { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
