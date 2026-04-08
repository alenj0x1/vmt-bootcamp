using Microsoft.AspNetCore.Mvc;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(IAuthService service) : ControllerBase
	{
		[HttpPost("login")]
		[EndpointSummary("Iniciar sesión como colaborador")]
		[EndpointDescription("Esto le permite iniciar sesión en el aplicativo. Genera dos tokens, uno que es el JWT para la autenticación con el aplicativo, y otro, que es, el que le permite realizar la renovación.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "jwt", "refresh_token")]
		public async Task<IActionResult> Login([FromBody] LoginAuthRequest model)
		{
			var srv = await service.Login(model);
			return Ok(srv);
		}

		[HttpPost("renew")]
		[EndpointSummary("Renovar sesión como colaborador")]
		[EndpointDescription("Esto le permite renovar la sesión en el aplicativo. Genera dos tokens, uno que es el JWT para la autenticación con el aplicativo, y otro, que es, el que le permite realizar la renovación.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "jwt", "refresh_token")]
		public async Task<IActionResult> Renew([FromBody] RenewAuthRequest model)
		{
			var srv = await service.Renew(model);
			return Ok(srv);
		}
	}
}
