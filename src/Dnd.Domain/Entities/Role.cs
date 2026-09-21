using System;
using System.Collections.Generic;

namespace Dnd.Domain.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Slug { get; set; } = null!;

    public string Descrizione { get; set; } = null!;

    public virtual ICollection<Campaignmember> Campaignmembers { get; set; } = new List<Campaignmember>();
}
