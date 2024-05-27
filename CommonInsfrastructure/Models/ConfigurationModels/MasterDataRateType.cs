using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataRateType
{
    public string RateId { get; set; } = null!;

    public string RateCategory { get; set; } = null!;

    public string RateType { get; set; } = null!;

    public string? Description { get; set; }

    public string ServiceType { get; set; } = null!;

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MasterDataRateTypeValue> MasterDataRateTypeValues { get; set; } = new List<MasterDataRateTypeValue>();

    public virtual MasterDataService ServiceTypeNavigation { get; set; } = null!;
}
