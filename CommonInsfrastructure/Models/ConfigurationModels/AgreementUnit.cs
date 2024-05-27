using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class AgreementUnit
{
    public long Id { get; set; }

    public string AgreementId { get; set; } = null!;

    public string UnitId { get; set; } = null!;

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AgreementInfo Agreement { get; set; } = null!;

    public virtual LocationMapUnit Unit { get; set; } = null!;
}
