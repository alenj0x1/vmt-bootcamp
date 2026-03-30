using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class Project
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Collaborator CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ProjectCollaborator> ProjectCollaborators { get; set; } = new List<ProjectCollaborator>();

    public virtual ICollection<ProjectMessage> ProjectMessages { get; set; } = new List<ProjectMessage>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
