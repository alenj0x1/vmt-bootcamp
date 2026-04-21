using Microsoft.EntityFrameworkCore;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Infrastructure.Persistence.SqlServer.Repositories
{
	public class CollaboratorRepository(TalentInsightsContext context) : GenericRepository<Collaborator>(context), ICollaboratorRepository
	{
		public async Task<bool> ClearRoles(List<CollaboratorRole> roles)
		{
			context.CollaboratorRoles.RemoveRange(roles);
			return true;
		}

		public async Task<Collaborator?> Get(Guid collaboratorId)
		{
			try
			{
				return await context.Collaborators
					.Include(collaborator => collaborator.CollaboratorRoleCollaborators)
					.ThenInclude(collaboratorRoles => collaboratorRoles.Role)
					.FirstOrDefaultAsync(x => x.Id == collaboratorId && x.DeletedAt == null);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<Collaborator?> Get(string email)
		{
			try
			{
				return await context.Collaborators
					.Include(collaborator => collaborator.CollaboratorRoleCollaborators)
					.ThenInclude(collaboratorRoles => collaboratorRoles.Role)
					.FirstOrDefaultAsync(x => x.Email == email && x.DeletedAt == null);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<Menu>> GetMenu(Guid collaboratorId)
		{
			var permissions = await context.CollaboratorRoles
				.Where(cr => cr.CollaboratorId == collaboratorId)
				.Join(context.RolePermissions,
					cr => cr.RoleId,
					rp => rp.RoleId,
					(cr, rp) => rp.PermissionId)
				.ToListAsync();


			return await context.Menus
				.Where(m => m.MenuPermissions.Any(mp => permissions.Contains(mp.PermissionId)) &&
					m.IsVisible &&
					m.IsActive)
				.OrderBy(m => m.ParentId)
				.ThenBy(m => m.SortOrder)
				.ToListAsync();
		}

		public async Task<Role?> GetRole(string name)
		{
			return await context.Roles.FirstOrDefaultAsync(x => x.Name == name);
		}

		public async Task<Role?> GetRole(Guid id)
		{
			return await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
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
	}
}
