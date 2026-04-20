using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Queries;
using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared;
using TalentInsights.Shared.Constants;
using TalentInsights.Shared.Helpers;

namespace TalentInsights.Application.Services
{
	public class CollaboratorService(IUnitOfWork uow, IConfiguration configuration, SMTP smtp, IEmailTemplateService emailTemplateService) : ICollaboratorService
	{
		public async Task<GenericResponse<CollaboratorDto>> Create(CreateCollaboratorRequest model, Claim? claim)
		{
			Role? roleToAssign = null;
			Collaborator? executor = null;

			// Normal use
			if (claim is not null)
			{
				executor = await GetExecutor(claim.Value);

				if (!model.RoleId.HasValue || model.RoleId.HasValue && model.RoleId.Value == Guid.Empty)
				{
					throw new NotFoundException(ValidationConstants.IsEmpty("RoleId"));
				}

				await ValidateEmailIfExists(model.Email);

				roleToAssign = await ValidateRole(executor, model.RoleId.Value);
			}
			// Without authentication for register
			else
			{
				roleToAssign = await uow.roleRepository.Get(x => x.Name == RoleConstants.Developer);
			}

			if (roleToAssign is null)
			{
				throw new BadRequestException("Imposible obtener el rol para asignarle al usuario");
			}

			var password = Generate.RandomText(32);

			var create = await uow.collaboratorRepository.Create(new Collaborator
			{
				GitlabProfile = model.GitlabProfile,
				FullName = model.FullName,
				Position = model.Position,
				Email = model.Email,
				Password = Hasher.HashPassword(password),
				CollaboratorRoleCollaborators = [
					new CollaboratorRole {
						RoleId = roleToAssign.Id,
						AssignedBy = executor?.Id
					}
				]
			});

			var template = await emailTemplateService.Get(EmailTemplateNameConstants.COLLABORATOR_REGISTER, new Dictionary<string, string>
			{
				{ "password", password }
			});
			await smtp.Send(model.Email, template.Subject, template.Body);
			await uow.SaveChangesAsync();

			return ResponseHelper.Create(Map(create));
		}

		public async Task<GenericResponse<bool>> Delete(Guid collaboratorId)
		{
			var collaborator = await GetCollaborator(collaboratorId);

			collaborator.DeletedAt = DateTimeHelper.UtcNow();
			await uow.collaboratorRepository.Update(collaborator);

			await uow.SaveChangesAsync();

			return ResponseHelper.Create(true);
		}

		public GenericResponse<List<CollaboratorDto>> Get(FilterColaboratorRequest model)
		{
			var queryable = uow.collaboratorRepository.Queryable();
			var collaborators = queryable
				.Include(collaborator => collaborator.CollaboratorRoleCollaborators)
				.ThenInclude(collaboratorRole => collaboratorRole.Role)
				.ApplyQuery(model)
				.AsQueryable()
				.Skip(model.Offset)
				.Take(model.Limit)
				.Select(collaborator => Map(collaborator))
				.ToList();

			return ResponseHelper.Create(collaborators, count: queryable.Count());
		}

		public async Task<GenericResponse<CollaboratorDto>> Get(Guid collaboratorId)
		{
			var collaborator = await GetCollaborator(collaboratorId);
			return ResponseHelper.Create(Map(collaborator));
		}

		public async Task<GenericResponse<CollaboratorDto>> Update(Guid collaboratorId, UpdateCollaboratorRequest model, Claim claim)
		{
			var executor = await GetExecutor(claim.Value);
			var collaborator = await GetCollaborator(collaboratorId);

			collaborator.GitlabProfile = model.GitlabProfile ?? collaborator.GitlabProfile;
			collaborator.Position = model.Position ?? collaborator.Position;
			collaborator.FullName = model.FullName ?? collaborator.FullName;

			if (!string.IsNullOrWhiteSpace(model.Email) && collaborator.Email != model.Email)
			{
				await ValidateEmailIfExists(model.Email);
				collaborator.Email = model.Email;
			}

			if (model.RoleId.HasValue)
			{
				var roleToAssign = await ValidateRole(executor, model.RoleId.Value);

				await uow.collaboratorRepository.ClearRoles([.. collaborator.CollaboratorRoleCollaborators]);

				collaborator.CollaboratorRoleCollaborators.Add(new CollaboratorRole
				{
					RoleId = roleToAssign.Id,
					AssignedBy = executor.Id
				});
			}

			collaborator.UpdatedAt = DateTimeHelper.UtcNow();

			var update = await uow.collaboratorRepository.Update(collaborator);

			await uow.SaveChangesAsync();

			return ResponseHelper.Create(Map(update));
		}

		private async Task<Collaborator> GetCollaborator(Guid collaboratorId)
		{
			return await uow.collaboratorRepository.Get(collaboratorId)
				?? throw new NotFoundException(ResponseConstants.COLLABORATOR_NOT_EXISTS);
		}

		private static CollaboratorDto Map(Collaborator collaborator)
		{
			var role = collaborator.CollaboratorRoleCollaborators.FirstOrDefault()?.Role;

			return new CollaboratorDto
			{
				CollaboratorId = collaborator.Id,
				FullName = collaborator.FullName,
				Position = collaborator.Position,
				GitlabProfile = collaborator.GitlabProfile,
				JoinedAt = collaborator.JoinedAt,
				CreatedAt = collaborator.CreatedAt,
				IsActive = collaborator.IsActive,
				Email = collaborator.Email,
				Role = role != null ? new RoleDto
				{
					Id = role.Id,
					Name = role.Name,
					Description = role.Description
				} : null
			};
		}

		public async Task CreateFirstUser()
		{
			var hasCreated = await uow.collaboratorRepository.HasCreated();
			if (hasCreated) return;

			var fullName = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_FULLNAME]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_FULLNAME));

			var email = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_EMAIL]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_EMAIL));

			var position = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_POSITION]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_POSITION));

			var password = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_PASSWORD]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_PASSWORD));

			var adminRole = await uow.roleRepository.Get(x => x.Name == RoleConstants.Admin)
				?? throw new Exception(ResponseConstants.RoleNotFound(RoleConstants.Admin));

			await uow.collaboratorRepository.Create(new Collaborator
			{
				FullName = fullName,
				Email = email,
				Position = position,
				Password = Hasher.HashPassword(password),
				CollaboratorRoleCollaborators = [
					new CollaboratorRole
					{
						RoleId = adminRole.Id,
					}
				]
			});

			await uow.SaveChangesAsync();
		}
		public async Task<Collaborator> GetExecutor(string value)
		{
			var uuid = Guid.Parse(value);
			return await uow.collaboratorRepository.Get(uuid)
				?? throw new NotFoundException(ResponseConstants.COLLABORATOR_NOT_EXISTS);
		}

		private async Task ValidateEmailIfExists(string email)
		{
			if (await uow.collaboratorRepository.IfExists(x => x.Email == email))
			{
				throw new BadRequestException(ResponseConstants.COLLABORATOR_EMAIL_TAKED);
			}
		}

		private async Task<Role> ValidateRole(Collaborator executor, Guid roleId)
		{
			var roleToAssign = await uow.roleRepository.Get(roleId)
				?? throw new NotFoundException(ResponseConstants.RoleNotFound(roleId));

			if (executor.CollaboratorRoleCollaborators.First().Role.Name == RoleConstants.HR && roleToAssign.Name == RoleConstants.Admin)
			{
				throw new BadRequestException(ResponseConstants.CANNOT_ASSIGN_THE_ROLE);
			}

			return roleToAssign;
		}

		public async Task<GenericResponse<CollaboratorDto>> Me(Claim claim)
		{
			var executor = await GetExecutor(claim.Value);
			return ResponseHelper.Create(Map(executor));
		}
	}
}
