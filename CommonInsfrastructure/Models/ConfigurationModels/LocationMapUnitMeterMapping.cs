using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class LocationMapUnitMeterMapping
{
    public long Id { get; set; }

    public string UnitId { get; set; } = null!;

    public string? UnitName { get; set; }

    public string MeterId { get; set; } = null!;

    public string? MeterName { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MasterDataMeter Meter { get; set; } = null!;
}
