using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface ICollaboratorRepository : IGenericRepository<Collaborator>
	{
		Task<Collaborator?> Get(Guid collaboratorId);
		Task<Collaborator?> Get(string email);
		Task<bool> IfExists(Guid collaboratorId);
		Task<bool> IfExists(string email);
		Task<bool> HasCreated();

		// Roles
		Task<Role?> GetRole(string name);
		Task<Role?> GetRole(Guid id);
	}
}
