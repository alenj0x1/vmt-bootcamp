using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Domain.Database.SqlServer
{
	public interface IUnitOfWork
	{
		ICollaboratorRepository collaboratorRepository { get; set; }
		Task SaveChangesAsync();
	}
}
