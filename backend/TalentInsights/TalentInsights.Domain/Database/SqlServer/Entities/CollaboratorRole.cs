using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class CollaboratorRole
{
    public Guid CollaboratorId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime AssignedAt { get; set; }

    public Guid? AssignedBy { get; set; }

    public virtual Collaborator? AssignedByNavigation { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
