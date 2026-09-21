using System;
using System.Collections.Generic;

namespace Dnd.Domain.Entities;

public partial class Character
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Class { get; set; } = null!;

    public int Level { get; set; }

    public int CurrentHp { get; set; }

    public int MaxHp { get; set; }

    public DateTime? CreatedAt { get; set; }
}
