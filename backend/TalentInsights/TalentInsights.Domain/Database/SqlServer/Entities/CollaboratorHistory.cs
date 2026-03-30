using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class CollaboratorHistory
{
    public Guid Id { get; set; }

    public Guid CollaboratorId { get; set; }

    public string EntityType { get; set; } = null!;

    public Guid EntityId { get; set; }

    public string EntityName { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public DateTime RecordedAt { get; set; }

    public virtual Collaborator Collaborator { get; set; } = null!;
}
