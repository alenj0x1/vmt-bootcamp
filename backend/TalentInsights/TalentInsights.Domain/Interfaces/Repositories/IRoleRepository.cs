using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface IRoleRepository : IGenericRepository<Role>
	{
		Task<Role?> Get(Guid id);
	}
}
