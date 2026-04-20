using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Requests.Auth.RecoverPassword;
using TalentInsights.Application.Models.Requests.Auth.Register;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared.Constants;
using TalentInsights.WebApi.Helpers;

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
		public async Task<GenericResponse<LoginAuthResponse>> Login([FromBody] LoginAuthRequest model)
		{
			var srv = await service.Login(model);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("renew")]
		[EndpointSummary("Renovar sesión como colaborador")]
		[EndpointDescription("Esto le permite renovar la sesión en el aplicativo. Genera dos tokens, uno que es el JWT para la autenticación con el aplicativo, y otro, que es, el que le permite realizar la renovación.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "jwt", "refresh_token")]
		public async Task<GenericResponse<LoginAuthResponse>> Renew([FromBody] RenewAuthRequest model)
		{
			var srv = await service.Renew(model);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("register/init")]
		[EndpointSummary("Comenzar proceso de registro")]
		[EndpointDescription("Le envía un correo electrónico a la persona, para que pueda registrarse en la plataforma.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "register")]
		public async Task<GenericResponse<string>> RegisterInit([FromBody] RegisterInitAuthRequest model)
		{
			var srv = await service.RegisterInit(model);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpGet("register/validate/{token}")]
		[EndpointSummary("Validar si el token existe o no expiró")]
		[EndpointDescription("Le permite validar si el token es correcto, para procesos de visualización de formularios.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "register", "validate")]
		public async Task<GenericResponse<RegisterInitAuthResponse>> RegisterValidateToken(string token)
		{
			var srv = await service.RegisterValidateToken(token);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("register/complete/{token}")]
		[EndpointSummary("Completar el proceso de registro")]
		[EndpointDescription("Le permite completar el proceso de registro. Por defecto, un colaborador al registrarse, tiene el rol de Developer.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "register", "complete")]
		public async Task<GenericResponse<CollaboratorDto>> RegisterValidateToken([FromBody] CreateCollaboratorRequest model, string token)
		{
			var srv = await service.RegisterComplete(model, token);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("recoverPassword")]
		[EndpointSummary("Envíar correo con OTP para recuperación de contraseña")]
		[EndpointDescription("Le permite recuperar la contraseña a un usuario mediante un código OTP.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "recover_password")]
		public async Task<GenericResponse<string>> RecoverPasswordSendOTP([FromBody] RecoverPasswordSendOTPAuthRequest model)
		{
			var srv = await service.RecoverPasswordSendOTP(model);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("recoverPassword/{code}")]
		[EndpointSummary("Recuperar ")]
		[EndpointDescription("Le permite recuperar la contraseña a un usuario.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "recover_password", "change_password")]
		public async Task<GenericResponse<string>> RecoverPassword([FromBody] RecoverPasswordAuthRequest model, string code)
		{
			var srv = await service.RecoverPassword(model, code);
			return ResponseStatus.Created(HttpContext, srv);
		}

		[HttpPost("changePassword")]
		[Authorize]
		[EndpointSummary("Cambiar la contraseña del usuario")]
		[EndpointDescription("Le permite cambiar la contraseña a un usuario.")]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status404NotFound)]
		[ProducesResponseType<GenericResponse<string>>(StatusCodes.Status200OK)]
		[Tags("auth", "collaborators", "recover_password", "change_password")]
		public async Task<GenericResponse<string>> ChangePassword([FromBody] ChangePasswordAuthRequest model)
		{
			var srv = await service.ChangePassword(model, UserClaim());
			return ResponseStatus.Created(HttpContext, srv);
		}

		private Claim UserClaim()
		{
			return User.FindFirst(ClaimsConstants.COLLABORATOR_ID)
				?? throw new BadRequestException(ResponseConstants.AUTH_CLAIM_USER_NOT_FOUND);
		}
	}
}
