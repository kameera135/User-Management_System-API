using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.OPCTagModels;

public partial class TblSchedule
{
    public int EventId { get; set; }

    public string OpcTag { get; set; } = null!;

    public DateTime OnActionTime { get; set; }

    public string OnActionValue { get; set; } = null!;

    public string OnActionStatus { get; set; } = null!;

    public DateTime? OnActionDoneTime { get; set; }

    public int? OnActionDoneStatus { get; set; }

    public DateTime OffActionTime { get; set; }

    public string OffActionValue { get; set; } = null!;

    public string OffActionStatus { get; set; } = null!;

    public DateTime? OffActionDoneTime { get; set; }

    public int? OffActionDoneStatus { get; set; }

    public string? ReferenceCode1 { get; set; }

    public string? ReferenceCode2 { get; set; }

    public string? ReferenceCode3 { get; set; }

    public string? OnCheckStatus { get; set; }

    public DateTime? OnCheckDoneTime { get; set; }

    public string? OffCheckStatus { get; set; }

    public DateTime? OffCheckDoneTime { get; set; }

    public DateTime? OracleInsertTime { get; set; }

    public DateTime? InsertTime { get; set; }
}
