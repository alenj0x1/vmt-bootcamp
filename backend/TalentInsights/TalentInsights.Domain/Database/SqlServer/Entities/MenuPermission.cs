using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class MenuPermission
{
    public Guid MenuId { get; set; }

    public Guid PermissionId { get; set; }

    public string MatchMode { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}
