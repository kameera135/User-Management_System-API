using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class LocationMapUnitControlTagMapping
{
    public long Id { get; set; }

    public string UnitId { get; set; } = null!;

    public string? UnitName { get; set; }

    public string ControlTag { get; set; } = null!;

    public bool ExtensionReady { get; set; }

    public string? Description { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
