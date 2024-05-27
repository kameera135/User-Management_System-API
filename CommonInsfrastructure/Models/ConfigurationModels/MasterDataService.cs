using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataService
{
    public string ServiceId { get; set; } = null!;

    public string ServiceType { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public double UnitConversionFactor { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MasterDataMeter> MasterDataMeters { get; set; } = new List<MasterDataMeter>();

    public virtual ICollection<MasterDataRateType> MasterDataRateTypes { get; set; } = new List<MasterDataRateType>();
}
