using System.Security.Claims;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Projects;
using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface IProjectService
	{
		Task<GenericResponse<ProjectDto>> Create(CreateProjectRequest model, Claim claim);
		Task<GenericResponse<List<ProjectDto>>> Get(FilterProjectRequest model);
	}
}
