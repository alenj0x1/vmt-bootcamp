using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Projects;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Queries;
using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Services
{
	public class ProjectService(IUnitOfWork uow, ICollaboratorService collaboratorService) : IProjectService
	{
		public async Task<GenericResponse<ProjectDto>> Create(CreateProjectRequest model, Claim claim)
		{
			var executor = await collaboratorService.GetExecutor(claim.Value);

			if (!ProjectsConstants.Statuses.Contains(model.Status))
			{
				throw new NotFoundException($"El estado de proyecto que argumentó no existe. Estados disponibles: {string.Join(", ", ProjectsConstants.Statuses)}");
			}

			if (await uow.projectRepository.IfExists(p => p.Name == model.Name))
			{
				throw new BadRequestException("Ya existe un proyecto con el nombre que argumentó");
			}

			var project = new Project
			{
				Name = model.Name,
				Description = model.Description,
				Status = model.Status,
				CreatedBy = executor.Id
			};
			await uow.projectRepository.Create(project);
			await uow.SaveChangesAsync();

			return ResponseHelper.Create(MapProject(project));
		}

		public async Task<GenericResponse<List<ProjectDto>>> Get(FilterProjectRequest model)
		{
			var queryable = uow.projectRepository.Queryable();
			var projects = queryable
				.Include(project => project.Teams)
				.ThenInclude(team => team.CreatedByNavigation)
				.Include(project => project.CreatedByNavigation)
				.ApplyQuery(model)
				.AsQueryable()
				.Skip(model.Offset)
				.Take(model.Limit)
				.ToList()
				.Select(project => MapProject(project))
				.ToList();

			return ResponseHelper.Create(projects, count: queryable.Count());
		}

		private ProjectDto MapProject(Project project)
		{
			var createdBy = project.CreatedByNavigation;

			return new ProjectDto
			{
				Id = project.Id,
				Name = project.Name,
				Description = project.Description,
				Status = project.Status,
				Teams = [.. project.Teams.Select(team =>
				{
					var createdByTeam = team.CreatedByNavigation;

					return new TeamDto
					{
						Id = team.Id,
						Name = team.Name,
						Description = team.Name,
						CreatedAt = team.CreatedAt,
						CreatedBy = new CollaboratorDto{
							CollaboratorId = createdByTeam.Id,
							FullName = createdByTeam.FullName,
							Email = createdByTeam.Email,
							IsActive = createdByTeam.IsActive,
							GitlabProfile = createdByTeam.GitlabProfile,
							Role = default!,
							JoinedAt = createdByTeam.JoinedAt
						},
						IsPublic = team.IsPublic,
						UpdatedAt = team.UpdatedAt,
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
