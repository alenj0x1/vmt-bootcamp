using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared.Constants;
using TalentInsights.WebApi.Attributes;
using TalentInsights.WebApi.Helpers;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[DeveloperAuthor(Name = "Alejandro M.", Description = "Esto lo cree para la información de la APP")]
	public class AppController(IAppService appService) : ControllerBase
	{
		[HttpGet("info")]
		[EndpointSummary("Información de la aplicación")]
		[EndpointDescription("Los roles, permisos, versión y mas detalles de la aplicación")]
		[ProducesResponseType<GenericResponse<AppInfoDto>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<AppInfoDto>> Info()
		{
			var srv = await appService.Info();
			return ResponseStatus.Ok(HttpContext, srv);
		}

		[HttpGet("menu")]
		[Authorize]
		[EndpointSummary("Menú de la aplicación")]
		[EndpointDescription("Las opciones que tiene el usuario, mediante su rol y permisos")]
		[ProducesResponseType<GenericResponse<AppInfoDto>>(StatusCodes.Status200OK)]
		public async Task<GenericResponse<List<MenuDto>>> Menu()
		{
			var srv = await appService.Menu(UserClaim());
			return ResponseStatus.Ok(HttpContext, srv);
		}

		private Claim UserClaim()
		{
			return User.FindFirst(ClaimsConstants.COLLABORATOR_ID)
				?? throw new BadRequestException(ResponseConstants.AUTH_CLAIM_USER_NOT_FOUND);
		}
	}
}
