using System;
using System.Collections.Generic;

namespace UMS.Api.Models;

public partial class Password
{
    public string PasswordId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? HashKey { get; set; }
}
