using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface ICollaboratorRepository : IGenericRepository<Collaborator>
	{
		Task<Collaborator?> Get(Guid collaboratorId);
		Task<Collaborator?> Get(string email);
		Task<bool> HasCreated();

		Task<bool> ClearRoles(List<CollaboratorRole> roles);
	}
}
