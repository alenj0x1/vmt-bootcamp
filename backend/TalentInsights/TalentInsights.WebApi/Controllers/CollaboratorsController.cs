using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared.Constants;
using TalentInsights.WebApi.Helpers;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CollaboratorsController(ICollaboratorService collaboratorService) : ControllerBase
	{
		[HttpPost]
		[Authorize(Roles = "Admin, HR")]
		[EndpointSummary("Crear un colaborador")]
		[EndpointDescription("Realiza la creación de un colaborador")]
		[ProducesResponseType<GenericResponse<CollaboratorDto>>(StatusCodes.Status201Created)]
		public async Task<GenericResponse<CollaboratorDto>> Create([FromBody] CreateCollaboratorRequest model)
		{
			var srv = await collaboratorService.Create(model, UserClaim());
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpGet]
		[Authorize]
		[EndpointSummary("Obtiene uno o más colaboradores")]
		[EndpointDescription("Realiza la petición para obtener uno o más colaboradores")]
		[ProducesResponseType<GenericResponse<List<CollaboratorDto>>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<List<CollaboratorDto>>> GetAll([FromQuery] FilterColaboratorRequest model, [FromHeader] string authorization)
		{
			var srv = collaboratorService.Get(model);
			return ResponseStatus.Ok(HttpContext, srv);
		}

		[HttpGet("{id:guid}")]
		[Authorize]
		[EndpointSummary("Obtener un colaborador")]
		[EndpointDescription("Obtiene la información de un colaborador")]
		[ProducesResponseType<GenericResponse<CollaboratorDto>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<CollaboratorDto>> GetById(Guid id)
		{
			var srv = await collaboratorService.Get(id);
			return ResponseStatus.Ok(HttpContext, srv);
		}

		[HttpGet("me")]
		[Authorize]
		[EndpointSummary("Obtiene al colaborador de la sesión actual")]
		[EndpointDescription("Obtiene la información del colaborador")]
		[ProducesResponseType<GenericResponse<CollaboratorDto>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<CollaboratorDto>> Me()
		{
			var srv = await collaboratorService.Me(UserClaim());
			return ResponseStatus.Ok(HttpContext, srv);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin, HR")]
		[EndpointSummary("Actualizar un colaborador")]
		[EndpointDescription("Actualiza la información de un colaborador")]
		[ProducesResponseType<GenericResponse<CollaboratorDto>>(StatusCodes.Status204NoContent)]
		public async Task<GenericResponse<CollaboratorDto>> Update([FromBody] UpdateCollaboratorRequest model, Guid id)
		{
			var srv = await collaboratorService.Update(id, model, UserClaim());
			return ResponseStatus.Updated(HttpContext, srv);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin, HR")]
		[EndpointSummary("Elimina un colaborador")]
		[EndpointDescription("Elimina un colaborador")]
		[ProducesResponseType<GenericResponse<bool>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<bool>> Delete(Guid id)
		{
			var srv = await collaboratorService.Delete(id);
			return ResponseStatus.Ok(HttpContext, srv);
		}

		private Claim UserClaim()
		{
			return User.FindFirst(ClaimsConstants.COLLABORATOR_ID)
				?? throw new BadRequestException(ResponseConstants.AUTH_CLAIM_USER_NOT_FOUND);
		}
	}
}
