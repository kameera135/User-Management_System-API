using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataPaymentTerm
{
    public string PaymentTermId { get; set; } = null!;

    public string PaymentTerm { get; set; } = null!;

    public int? DefaultCreditPeriod { get; set; }

    public int? DefaultLatePeriod { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
