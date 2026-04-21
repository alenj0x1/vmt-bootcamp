using TalentInsights.Application.Models.Requests.Projects;
using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Application.Queries
{
	public static class ProjectFilterQuery
	{
		public static IQueryable<Project> ApplyQuery(this IQueryable<Project> queryable, FilterProjectRequest model)
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

			if (!string.IsNullOrWhiteSpace(model.Status))
			{
				queryable = queryable.Where(x => x.Status.Contains(model.Status));
			}

			return queryable;
		}
	}
}
