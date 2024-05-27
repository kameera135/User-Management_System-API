using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class LocationMapUnit
{
    public string UnitId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ParentLevelId { get; set; } = null!;

    public bool HasDefaultWorkingHours { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AgreementUnit> AgreementUnits { get; set; } = new List<AgreementUnit>();

    public virtual ICollection<LocationMapUnitTenantMapping> LocationMapUnitTenantMappings { get; set; } = new List<LocationMapUnitTenantMapping>();

    public virtual ICollection<LocationMapUnitUserMapping> LocationMapUnitUserMappings { get; set; } = new List<LocationMapUnitUserMapping>();

    public virtual ICollection<LocationMapUnitWorkingHour> LocationMapUnitWorkingHours { get; set; } = new List<LocationMapUnitWorkingHour>();

    public virtual LocationMapLevel ParentLevel { get; set; } = null!;
}
