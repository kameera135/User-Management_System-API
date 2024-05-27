using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class AgreementRate
{
    public long Id { get; set; }

    public string AgreementId { get; set; } = null!;

    public string Service { get; set; } = null!;

    public string Rate { get; set; } = null!;

    public string? RateType { get; set; }

    public bool? UseGlobalValue { get; set; }

    public int? NormalValue { get; set; }

    public int? ExtendedValue { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AgreementInfo Agreement { get; set; } = null!;
}
