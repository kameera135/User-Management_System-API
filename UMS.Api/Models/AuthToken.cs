using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class AuthToken
{
    public long TokenId { get; set; }

    public string? Token { get; set; }

    public long UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ExpireDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual User User { get; set; } = null!;
}
