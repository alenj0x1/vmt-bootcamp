using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class ProjectMessage
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid CollaboratorId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
