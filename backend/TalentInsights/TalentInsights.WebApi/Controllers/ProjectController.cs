using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Projects;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared.Constants;
using TalentInsights.WebApi.Helpers;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectController(IProjectService service) : ControllerBase
	{
		[HttpPost]
		[Authorize(Roles = "Admin, TeamLeader")]
		[EndpointSummary("Crear un proyecto")]
		[EndpointDescription("Esto le permite crear un proyecto, para poder agregar equipos")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
		[Tags("projects", "create")]
		public async Task<GenericResponse<ProjectDto>> Create([FromBody] CreateProjectRequest model)
		{
			var srv = await service.Create(model, UserClaim());
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpGet]
		[Authorize(Roles = "Admin, TeamLeader")]
		[EndpointSummary("Obtener uno o más proyectos")]
		[EndpointDescription("Esto le permite obtener la información de uno o más proyectos")]
		[ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
		[Tags("projects", "filter")]
		public async Task<GenericResponse<List<ProjectDto>>> Get([FromQuery] FilterProjectRequest model)
		{
			var srv = await service.Get(model);
			return ResponseStatus.Created(HttpContext, srv);
		}

		private Claim UserClaim()
		{
			return User.FindFirst(ClaimsConstants.COLLABORATOR_ID)
				?? throw new BadRequestException(ResponseConstants.AUTH_CLAIM_USER_NOT_FOUND);
		}
	}
}
