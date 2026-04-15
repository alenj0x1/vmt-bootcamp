using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer
{
	public class UnitOfWork(TalentInsightsContext context, ICollaboratorRepository collaboratorsRepository) : IUnitOfWork
	{
		private readonly TalentInsightsContext _context = context;
		public ICollaboratorRepository collaboratorRepository { get; set; } = collaboratorsRepository;

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
