using Microsoft.Extensions.Configuration;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Domain.Interfaces.Repositories;
using TalentInsights.Shared;
using TalentInsights.Shared.Constants;
using TalentInsights.Shared.Helpers;

namespace TalentInsights.Application.Services
{
	public class CollaboratorService(ICollaboratorRepository repository, IConfiguration configuration) : ICollaboratorService
	{
		public async Task<GenericResponse<CollaboratorDto>> Create(CreateCollaboratorRequest model)
		{
			var create = await repository.Create(new Collaborator
			{
				GitlabProfile = model.GitlabProfile,
				FullName = model.FullName,
				Position = model.Position
			});

			return ResponseHelper.Create(Map(create));
		}

		public async Task<GenericResponse<bool>> Delete(Guid collaboratorId)
		{
			var collaborator = await GetCollaborator(collaboratorId);

			collaborator.DeletedAt = DateTimeHelper.UtcNow();
			await repository.Update(collaborator);

			return ResponseHelper.Create(true);
		}

		public GenericResponse<List<CollaboratorDto>> Get(FilterColaboratorRequest model)
		{
			var queryable = repository.Queryable();

			// Filtrado de nombre
			if (!string.IsNullOrWhiteSpace(model.FullName))
			{
				queryable = queryable.Where(x => x.FullName.Contains(model.FullName ?? ""));
			}

			// Filtrado de perfil de gitlab
			if (!string.IsNullOrWhiteSpace(model.GitlabProfile))
			{
				queryable = queryable.Where(x => x.GitlabProfile != null && x.GitlabProfile.Contains(model.GitlabProfile ?? ""));
			}

			// Filtrado del cargo
			if (!string.IsNullOrWhiteSpace(model.Position))
			{
				queryable = queryable.Where(x => x.Position.Contains(model.Position ?? ""));
			}

			// Realizar paginación y realizar consulta
			var collaborators = queryable.Skip(model.Offset).Take(model.Limit).ToList();

			// Mapear colaboradores
			List<CollaboratorDto> mapped = [];
			foreach (var collaborator in collaborators)
			{
				mapped.Add(Map(collaborator));
			}

			return ResponseHelper.Create(mapped);
		}

		public async Task<GenericResponse<CollaboratorDto>> Get(Guid collaboratorId)
		{
			var collaborator = await GetCollaborator(collaboratorId);
			return ResponseHelper.Create(Map(collaborator));
		}

		public async Task<GenericResponse<CollaboratorDto>> Update(Guid collaboratorId, UpdateCollaboratorRequest model)
		{
			var collaborator = await GetCollaborator(collaboratorId);

			collaborator.GitlabProfile = model.GitlabProfile ?? collaborator.GitlabProfile;
			collaborator.Position = model.Position ?? collaborator.Position;
			collaborator.FullName = model.FullName ?? collaborator.FullName;

			collaborator.UpdatedAt = DateTimeHelper.UtcNow();

			var update = await repository.Update(collaborator);

			return ResponseHelper.Create(Map(update));
		}

		private async Task<Collaborator> GetCollaborator(Guid collaboratorId)
		{
			return await repository.Get(collaboratorId)
				?? throw new NotFoundException(ResponseConstants.COLLABORATOR_NOT_EXISTS);
		}

		private static CollaboratorDto Map(Collaborator collaborator)
		{
			return new CollaboratorDto
			{
				CollaboratorId = collaborator.Id,
				FullName = collaborator.FullName,
				Position = collaborator.Position,
				GitlabProfile = collaborator.GitlabProfile,
				JoinedAt = collaborator.JoinedAt,
				CreatedAt = collaborator.CreatedAt,
				IsActive = collaborator.IsActive
			};
		}

		public async Task CreateFirstUser()
		{
			var hasCreated = await repository.HasCreated();
			if (hasCreated) return;

			var fullName = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_FULLNAME]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_FULLNAME));

			var email = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_EMAIL]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_EMAIL));

			var position = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_POSITION]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_POSITION));

			var password = configuration[ConfigurationConstants.FIRST_APP_TIME_USER_PASSWORD]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.FIRST_APP_TIME_USER_PASSWORD));

			await repository.Create(new Collaborator
			{
				FullName = fullName,
				Email = email,
				Position = position,
				Password = Hasher.HashPassword(password)
			});
		}
	}
}
