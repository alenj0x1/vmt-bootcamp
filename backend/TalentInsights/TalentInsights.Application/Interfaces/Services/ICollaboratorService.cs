using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface ICollaboratorService
	{
		public Task<GenericResponse<CollaboratorDto>> Create(CreateCollaboratorRequest model);
		public Task<GenericResponse<CollaboratorDto>> Update(Guid collaboratorId, UpdateCollaboratorRequest model);
		public GenericResponse<List<CollaboratorDto>> Get(FilterColaboratorRequest model);
		public Task<GenericResponse<CollaboratorDto>> Get(Guid collaboratorId);
		public Task<GenericResponse<bool>> Delete(Guid collaboratorId);
		public Task CreateFirstUser();
	}
}
