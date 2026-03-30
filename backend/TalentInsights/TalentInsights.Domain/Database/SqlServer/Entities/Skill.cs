using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class Skill
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public virtual ICollection<CollaboratorSkill> CollaboratorSkills { get; set; } = new List<CollaboratorSkill>();
}
