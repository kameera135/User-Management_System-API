using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataTaxType
{
    public string TaxId { get; set; } = null!;

    public string TaxCategory { get; set; } = null!;

    public string TaxType { get; set; } = null!;

    public int Rank { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MasterDataTaxTypeValue> MasterDataTaxTypeValues { get; set; } = new List<MasterDataTaxTypeValue>();
}
