using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class CollaboratorSkill
{
    public Guid CollaboratorId { get; set; }

    public Guid SkillId { get; set; }

    public DateTime AcquiredAt { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
