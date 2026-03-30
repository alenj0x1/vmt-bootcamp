using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class Menu
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public string IconName { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Menu> InverseParent { get; set; } = new List<Menu>();

    public virtual ICollection<MenuPermission> MenuPermissions { get; set; } = new List<MenuPermission>();

    public virtual Menu? Parent { get; set; }
}
