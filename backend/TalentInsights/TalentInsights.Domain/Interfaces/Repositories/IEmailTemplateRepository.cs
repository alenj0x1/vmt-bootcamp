using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
	public interface IEmailTemplateRepository
	{
		Task<List<EmailTemplate>> Get();
	}
}
