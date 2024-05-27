using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class TenantInfo
{
    public string TenantId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AgreementInfo> AgreementInfos { get; set; } = new List<AgreementInfo>();

    public virtual ICollection<LocationMapUnitTenantMapping> LocationMapUnitTenantMappings { get; set; } = new List<LocationMapUnitTenantMapping>();
}
