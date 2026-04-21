using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer.Repositories
{
	public class ProjectRepository(TalentInsightsContext context) : GenericRepository<Project>(context), IProjectRepository
	{
	}
}
