using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataRateTypeValue
{
    public long Id { get; set; }

    public string Rate { get; set; } = null!;

    public DateTime EffectiveDate { get; set; }

    public double NormalValue { get; set; }

    public double ExtendedValue { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MasterDataRateType RateNavigation { get; set; } = null!;
}
