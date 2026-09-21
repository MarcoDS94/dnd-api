using System;
using System.Collections.Generic;

namespace Dnd.Domain.Entities;

public partial class Campaignmember
{
    public Guid Campaignid { get; set; }

    public Guid Userid { get; set; }

    public int Roleid { get; set; }

    public DateTime Joinedat { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
