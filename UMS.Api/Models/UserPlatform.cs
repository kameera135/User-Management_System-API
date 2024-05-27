using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class UserPlatform
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PlatformId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual Platform Platform { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
