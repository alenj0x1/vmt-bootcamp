using Microsoft.AspNetCore.Mvc;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Responses;
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
	}
}
