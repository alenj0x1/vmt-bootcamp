using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Teams;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared.Constants;
using TalentInsights.WebApi.Helpers;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TeamController(ITeamService service) : ControllerBase
	{
		[HttpPost]
		[Authorize(Roles = "Admin, TeamLeader")]
		[EndpointSummary("Crear un equipo")]
		[EndpointDescription("Esto le permite crear un equipo, para poder agregar miembros")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<TeamDto>>(StatusCodes.Status200OK)]
		[Tags("teams", "create")]
		public async Task<GenericResponse<TeamDto>> Create([FromBody] CreateTeamRequest model)
		{
			var srv = await service.Create(model, UserClaim());
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpGet]
		[Authorize(Roles = "Admin, TeamLeader")]
		[EndpointSummary("Obtener uno o más proyectos")]
		[EndpointDescription("Esto le permite obtener la información de uno o más proyectos")]
		[ProducesResponseType<GenericResponse<TeamDto>>(StatusCodes.Status200OK)]
		[Tags("teams", "filter")]
		public async Task<GenericResponse<List<TeamDto>>> Get([FromQuery] FilterTeamRequest model)
		{
			var srv = await service.Get(model);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("{teamId:guid}/members/{collaboratorId:guid}")]
		[Authorize(Roles = "Admin, TeamLeader")]
		[EndpointSummary("Añadir a un colaborador a un equipo")]
		[EndpointDescription("Esto le permite añadir un colaborador al equipo")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<TeamDto>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<TeamDto>> AddMember(Guid teamId, Guid collaboratorId)
		{
			var srv = await service.AddMember(collaboratorId, teamId);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpDelete("{teamId:guid}/members/{collaboratorId:guid}")]
		[Authorize(Roles = "Admin, TeamLeader")]
		[EndpointSummary("Remover un miembro de un equipo")]
		[EndpointDescription("Esto le permite remover un miembro del equipo")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<TeamDto>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<TeamDto>> RemoveMember(Guid teamId, Guid collaboratorId)
		{
			var srv = await service.RemoveMember(collaboratorId, teamId);
			return ResponseStatus.Created(HttpContext, srv);
		}

		private Claim UserClaim()
		{
			return User.FindFirst(ClaimsConstants.COLLABORATOR_ID)
				?? throw new BadRequestException(ResponseConstants.AUTH_CLAIM_USER_NOT_FOUND);
		}
	}
}
