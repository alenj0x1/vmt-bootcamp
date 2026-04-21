using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer.Repositories
{
	public class TeamRepository(TalentInsightsContext context) : GenericRepository<Team>(context), ITeamRepository
	{
		public async Task AddMember(Guid collaboratorId, Guid teamId)
		{
			await context.TeamMembers.AddAsync(new TeamMember
			{
				TeamId = teamId,
				CollaboratorId = collaboratorId
			});
		}

		public async Task<TeamMember?> GetMember(Guid collaboratorId, Guid teamId)
		{
			return await context.TeamMembers.Where(x => x.CollaboratorId == collaboratorId &&
				x.TeamId == teamId).FirstOrDefaultAsync();
		}

		public async Task RemoveMember(TeamMember teamMember)
		{
			context.TeamMembers.Remove(teamMember);
		}

		public async override Task<Team?> Get(Expression<Func<Team, bool>> expression)
		{
			return await context.Teams
				.Include(team => team.TeamMembers)
				.ThenInclude(member => member.Collaborator)
				.Include(team => team.CreatedByNavigation)
				.FirstOrDefaultAsync(expression);
		}
	}
}
