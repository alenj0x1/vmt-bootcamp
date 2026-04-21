using System.Security.Claims;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Teams;
using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface ITeamService
	{
		Task<GenericResponse<TeamDto>> Create(CreateTeamRequest model, Claim claim);
		Task<GenericResponse<List<TeamDto>>> Get(FilterTeamRequest model);
		Task<GenericResponse<TeamDto>> AddMember(Guid collaboratorId, Guid teamId);
		Task<GenericResponse<TeamDto>> RemoveMember(Guid collaboratorId, Guid teamId);
	}
}
