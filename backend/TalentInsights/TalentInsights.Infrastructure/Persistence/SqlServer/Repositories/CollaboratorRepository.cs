using Microsoft.EntityFrameworkCore;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer.Repositories
{
	public class CollaboratorRepository(TalentInsightsContext context) : ICollaboratorRepository
	{
		public async Task<Collaborator> Create(Collaborator collaborator)
		{
			try
			{
				// insert
				await context.Collaborators.AddAsync(collaborator);

				// execution // commit
				await context.SaveChangesAsync();

				return collaborator;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<Collaborator?> Get(Guid collaboratorId)
		{
			try
			{
				return await context.Collaborators.FirstOrDefaultAsync(x => x.Id == collaboratorId && x.DeletedAt == null);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<bool> HasCreated()
		{
			try
			{
				return await context.Collaborators.AnyAsync();
			}
			catch
			{
				throw;
			}
		}

		public async Task<bool> IfExists(Guid collaboratorId)
		{
			try
			{
				return await context.Collaborators.AnyAsync(x => x.Id == collaboratorId);
			}
			catch (Exception)
			{

				throw;
			}
		}

		public IQueryable<Collaborator> Queryable()
		{
			try
			{
				return context.Collaborators.Where(x => x.DeletedAt == null).AsQueryable();
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<Collaborator> Update(Collaborator collaborator)
		{
			try
			{
				context.Collaborators.Update(collaborator);
				await context.SaveChangesAsync();

				return collaborator;
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
