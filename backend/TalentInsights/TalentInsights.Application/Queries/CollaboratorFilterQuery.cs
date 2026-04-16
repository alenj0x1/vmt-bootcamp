using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Application.Queries
{
	public static class CollaboratorFilterQuery
	{
		public static IQueryable<Collaborator> ApplyQuery(this IQueryable<Collaborator> queryable, FilterColaboratorRequest model)
		{
			// Filtrado de nombre
			if (!string.IsNullOrWhiteSpace(model.FullName))
			{
				queryable = queryable.Where(x => x.FullName.Contains(model.FullName ?? ""));
			}

			// Filtrado de perfil de gitlab
			if (!string.IsNullOrWhiteSpace(model.GitlabProfile))
			{
				queryable = queryable.Where(x => x.GitlabProfile != null && x.GitlabProfile.Contains(model.GitlabProfile ?? ""));
			}

			// Filtrado del cargo
			if (!string.IsNullOrWhiteSpace(model.Position))
			{
				queryable = queryable.Where(x => x.Position.Contains(model.Position ?? ""));
			}

			return queryable;
		}
	}
}
