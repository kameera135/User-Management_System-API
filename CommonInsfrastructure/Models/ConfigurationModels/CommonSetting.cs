using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class CommonSetting
{
    public string Setting { get; set; } = null!;

    public bool Value { get; set; }

    public string? Data { get; set; }

    public string? Description { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
