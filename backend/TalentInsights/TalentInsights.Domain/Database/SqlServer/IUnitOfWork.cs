using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Domain.Database.SqlServer
{
	public interface IUnitOfWork
	{
		ICollaboratorRepository collaboratorRepository { get; set; }
		IEmailTemplateRepository emailTemplateRepository { get; set; }
		IRoleRepository roleRepository { get; set; }
		IProjectRepository projectRepository { get; set; }
		ITeamRepository teamRepository { get; set; }
		Task SaveChangesAsync();
	}
}
