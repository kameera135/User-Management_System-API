using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class LocationMapLevel
{
    public string LevelId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string BuildingId { get; set; } = null!;

    public string? ParentId { get; set; }

    public string? Icon { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual LocationMapBuilding Building { get; set; } = null!;

    public virtual ICollection<LocationMapLevel> InverseParent { get; set; } = new List<LocationMapLevel>();

    public virtual ICollection<LocationMapUnit> LocationMapUnits { get; set; } = new List<LocationMapUnit>();

    public virtual LocationMapLevel? Parent { get; set; }
}
