using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Teams;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Queries;
using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Exceptions;

namespace TalentInsights.Application.Services
{
	public class TeamService(IUnitOfWork uow, ICollaboratorService collaboratorService) : ITeamService
	{
		public async Task<GenericResponse<TeamDto>> Create(CreateTeamRequest model, Claim claim)
		{
			var executor = await collaboratorService.GetExecutor(claim.Value);

			if (await uow.projectRepository.IfExists(p => p.Name == model.Name))
			{
				throw new BadRequestException("Ya existe un equipo con el nombre que argumentó");
			}

			var team = new Team
			{
				Name = model.Name,
				Description = model.Description,
				CreatedBy = executor.Id
			};
			await uow.teamRepository.Create(team);
			await uow.SaveChangesAsync();

			return ResponseHelper.Create(MapTeam(team));
		}

		public async Task<GenericResponse<List<TeamDto>>> Get(FilterTeamRequest model)
		{
			var queryable = uow.teamRepository.Queryable();
			var teams = queryable
				.Include(team => team.TeamMembers)
				.ThenInclude(member => member.Collaborator)
				.Include(team => team.CreatedByNavigation)
				.ApplyQuery(model)
				.AsQueryable()
				.Skip(model.Offset)
				.Take(model.Limit)
				.ToList()
				.Select(team => MapTeam(team))
				.ToList();

			return ResponseHelper.Create(teams, count: queryable.Count());
		}

		public async Task<GenericResponse<TeamDto>> AddMember(Guid collaboratorId, Guid teamId)
		{
			if (!await uow.teamRepository.IfExists(x => x.Id == teamId))
			{
				throw new NotFoundException("El equipo que argumentó no existe");
			}

			if (!await uow.collaboratorRepository.IfExists(x => x.Id == collaboratorId))
			{
				throw new NotFoundException("El colaborador que argumentó no existe");
			}

			if (await uow.teamRepository.GetMember(collaboratorId, teamId) is not null)
			{
				throw new BadRequestException("El colaborador ya es miembro del equipo que argumentó");
			}

			await uow.teamRepository.AddMember(collaboratorId, teamId);
			await uow.SaveChangesAsync();

			var team = await uow.teamRepository.Get(x => x.Id == teamId)
				?? throw new BadRequestException("Imposible obtener los datos del equipo");

			return ResponseHelper.Create(MapTeam(team));
		}

		public async Task<GenericResponse<TeamDto>> RemoveMember(Guid collaboratorId, Guid teamId)
		{
			if (!await uow.teamRepository.IfExists(x => x.Id == teamId))
			{
				throw new NotFoundException("El equipo que argumentó no existe");
			}

			if (!await uow.collaboratorRepository.IfExists(x => x.Id == collaboratorId))
			{
				throw new NotFoundException("El colaborador que argumentó no existe");
			}

			var member = await uow.teamRepository.GetMember(collaboratorId, teamId)
				?? throw new BadRequestException("El colaborador no es miembro del equipo que argumentó");

			await uow.teamRepository.RemoveMember(member);
			await uow.SaveChangesAsync();

			var team = await uow.teamRepository.Get(x => x.Id == teamId)
				?? throw new BadRequestException("Imposible obtener los datos del equipo");

			return ResponseHelper.Create(MapTeam(team));
		}

		private TeamDto MapTeam(Team team)
		{
			var createdBy = team.CreatedByNavigation;

			return new TeamDto
			{
				Id = team.Id,
				Name = team.Name,
				Description = team.Description,
				Members = [.. team.TeamMembers.Select(team =>
				{
					var collaborator = team.Collaborator;

					return new CollaboratorDto{
						CollaboratorId = collaborator.Id,
						FullName = collaborator.FullName,
						Email = collaborator.Email,
						IsActive = collaborator.IsActive,
						GitlabProfile = collaborator.GitlabProfile,
						Role = default!,
						JoinedAt = collaborator.JoinedAt
					};
				})],
				CreatedBy = new CollaboratorDto
				{
					CollaboratorId = createdBy.Id,
					FullName = createdBy.FullName,
					Email = createdBy.Email,
					IsActive = createdBy.IsActive,
					GitlabProfile = createdBy.GitlabProfile,
					Role = default!,
					JoinedAt = createdBy.JoinedAt
				}
			};
		}
	}
}
