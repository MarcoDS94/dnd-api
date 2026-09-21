using System;
using System.Collections.Generic;

namespace Dnd.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public virtual ICollection<Campaignmember> Campaignmembers { get; set; } = new List<Campaignmember>();
}
