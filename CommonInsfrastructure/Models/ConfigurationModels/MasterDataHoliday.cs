using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataHoliday
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
