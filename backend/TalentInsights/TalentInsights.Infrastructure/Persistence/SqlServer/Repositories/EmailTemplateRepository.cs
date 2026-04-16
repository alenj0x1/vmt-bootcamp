using Microsoft.EntityFrameworkCore;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer.Repositories
{
	public class EmailTemplateRepository(TalentInsightsContext context) : IEmailTemplateRepository
	{
		public async Task<List<EmailTemplate>> Get()
		{
			return await context.EmailTemplates.ToListAsync();
		}
	}
}
