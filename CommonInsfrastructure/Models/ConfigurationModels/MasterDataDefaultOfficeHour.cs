using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class MasterDataDefaultOfficeHour
{
    public long Id { get; set; }

    public string Day { get; set; } = null!;

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
