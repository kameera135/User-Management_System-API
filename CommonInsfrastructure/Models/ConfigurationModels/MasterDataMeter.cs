using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataMeter
{
    public string MeterId { get; set; } = null!;

    public string MeterName { get; set; } = null!;

    public string ServiceType { get; set; } = null!;

    public string? MeterType { get; set; }

    public string? MeterTag { get; set; }

    public double MaxValue { get; set; }

    public string MeterUnit { get; set; } = null!;

    public double? Factor { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<LocationMapUnitMeterMapping> LocationMapUnitMeterMappings { get; set; } = new List<LocationMapUnitMeterMapping>();

    public virtual MasterDataService ServiceTypeNavigation { get; set; } = null!;
}
