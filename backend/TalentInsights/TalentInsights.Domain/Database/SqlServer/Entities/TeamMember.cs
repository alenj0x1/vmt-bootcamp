using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class TeamMember
{
    public Guid TeamId { get; set; }

    public Guid CollaboratorId { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
