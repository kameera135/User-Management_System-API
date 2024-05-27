using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class LocationMapBuilding
{
    public string BuildingId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<LocationMapLevel> LocationMapLevels { get; set; } = new List<LocationMapLevel>();
}
