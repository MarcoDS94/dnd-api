using System;
using System.Collections.Generic;

namespace Dnd.Domain.Entities;

public partial class Campaign
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Coverimageurl { get; set; }

    public string CodInd { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public virtual ICollection<Campaignmember> Campaignmembers { get; set; } = new List<Campaignmember>();
}
