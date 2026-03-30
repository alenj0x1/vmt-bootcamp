using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class ProjectCollaborator
{
    public Guid ProjectId { get; set; }

    public Guid CollaboratorId { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
