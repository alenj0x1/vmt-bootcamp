using Microsoft.EntityFrameworkCore;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer.Repositories
{
	public class RoleRepository(TalentInsightsContext context) : GenericRepository<Role>(context), IRoleRepository
	{
		public async Task<Role?> Get(Guid id)
		{
			return await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
		}
	}
}
