using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class Permission
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Module { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Specificity { get; set; } = null!;

    public virtual ICollection<CollaboratorPermission> CollaboratorPermissions { get; set; } = new List<CollaboratorPermission>();

    public virtual ICollection<MenuPermission> MenuPermissions { get; set; } = new List<MenuPermission>();
}
