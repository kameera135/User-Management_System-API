using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.EmailModels;

public partial class TblEmailList
{
    public int EmailId { get; set; }

    public string? ToList { get; set; }

    public string? CcList { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public string? AttachmentList { get; set; }

    public string? Status { get; set; }

    public DateTime? GeneratedTime { get; set; }

    public DateTime? SendTime { get; set; }

    public string? BookingCode { get; set; }

    public string? Type { get; set; }
}
