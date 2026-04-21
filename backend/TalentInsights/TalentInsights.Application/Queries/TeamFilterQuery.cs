using TalentInsights.Application.Models.Requests.Teams;
using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Application.Queries
{
	public static class TeamFilterQuery
	{
		public static IQueryable<Team> ApplyQuery(this IQueryable<Team> queryable, FilterTeamRequest model)
		{
			if (model.Id.HasValue)
			{
				queryable = queryable.Where(x => x.Id == model.Id.Value);
			}

			if (!string.IsNullOrWhiteSpace(model.Name))
			{
				queryable = queryable.Where(x => x.Name.Contains(model.Name));
			}

			if (!string.IsNullOrWhiteSpace(model.Description))
			{
				queryable = queryable.Where(x => x.Description != null && x.Description.Contains(model.Description));
			}

			return queryable;
		}
	}
}
