using System;
using System.Collections.Generic;

namespace TalentInsights.Domain.Database.SqlServer.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string Code { get; set; } = null!;

    public string ShowName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
