using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class CollaboratorPermission
{
    public Guid CollaboratorId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTime AssignedAt { get; set; }

    public Guid? AssignedBy { get; set; }

    public virtual Collaborator? AssignedByNavigation { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}
