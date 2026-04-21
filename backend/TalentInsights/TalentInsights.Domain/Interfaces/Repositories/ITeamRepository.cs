using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface ITeamRepository : IGenericRepository<Team>
	{
		Task AddMember(Guid collaboratorId, Guid teamId);
		Task<TeamMember?> GetMember(Guid collaboratorId, Guid teamId);
		Task RemoveMember(TeamMember teamMember);
	}
}
