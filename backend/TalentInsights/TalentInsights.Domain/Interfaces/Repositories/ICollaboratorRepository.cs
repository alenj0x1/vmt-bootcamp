using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface ICollaboratorRepository
	{
		Task<Collaborator> Create(Collaborator collaborator);
		Task<Collaborator?> Get(Guid collaboratorId);
		Task<Collaborator?> Get(string email);
		IQueryable<Collaborator> Queryable();
		Task<bool> IfExists(Guid collaboratorId);
		Task<Collaborator> Update(Collaborator collaborator);
		Task<bool> HasCreated();

		// Roles
		Task<Role?> GetRole(string name);
		Task<Role?> GetRole(Guid id);
	}
}
