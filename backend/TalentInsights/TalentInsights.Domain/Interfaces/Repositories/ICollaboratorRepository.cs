using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface ICollaboratorRepository
	{
		Task<Collaborator> Create(Collaborator collaborator);
		Task<Collaborator?> Get(Guid collaboratorId);
		IQueryable<Collaborator> Queryable();
		Task<bool> IfExists(Guid collaboratorId);
		Task<Collaborator> Update(Collaborator collaborator);
		Task<bool> HasCreated();
	}
}
