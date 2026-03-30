using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class Collaborator
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? GitlabProfile { get; set; }

    public string Position { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CollaboratorHistory> CollaboratorHistories { get; set; } = new List<CollaboratorHistory>();

    public virtual ICollection<CollaboratorPermission> CollaboratorPermissionAssignedByNavigations { get; set; } = new List<CollaboratorPermission>();

    public virtual ICollection<CollaboratorPermission> CollaboratorPermissionCollaborators { get; set; } = new List<CollaboratorPermission>();

    public virtual ICollection<CollaboratorSkill> CollaboratorSkills { get; set; } = new List<CollaboratorSkill>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<ProjectCollaborator> ProjectCollaborators { get; set; } = new List<ProjectCollaborator>();

    public virtual ICollection<ProjectMessage> ProjectMessages { get; set; } = new List<ProjectMessage>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
