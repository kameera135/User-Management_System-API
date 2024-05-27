using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataTaxTypeValue
{
    public long Id { get; set; }

    public string Tax { get; set; } = null!;

    public DateTime EffectiveDate { get; set; }

    public double Value { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MasterDataTaxType TaxNavigation { get; set; } = null!;
}
