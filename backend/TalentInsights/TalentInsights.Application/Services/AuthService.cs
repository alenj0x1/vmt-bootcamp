using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Helpers;
using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Requests.Auth.RecoverPassword;
using TalentInsights.Application.Models.Requests.Auth.Register;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;
using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared;
using TalentInsights.Shared.Constants;
using TalentInsights.Shared.Helpers;

namespace TalentInsights.Application.Services
{
	public class AuthService(
		IUnitOfWork uow,
		IConfiguration configuration,
		ICacheService cacheService,
		IEmailTemplateService emailTemplateService,
		SMTP smtp,
		ICollaboratorService collaboratorService
		) : IAuthService
	{
		public async Task<GenericResponse<LoginAuthResponse>> Login(LoginAuthRequest model)
		{
			var collaborator = await uow.collaboratorRepository.Get(model.Email)
				?? throw new BadRequestException(ResponseConstants.AUTH_USER_OR_PASSWORD_NOT_FOUND);

			var validatePassword = Hasher.ComparePassword(model.Password, collaborator.Password);
			if (!validatePassword)
			{
				var templateFailed = await emailTemplateService.Get(EmailTemplateNameConstants.AUTH_LOGIN_FAILED, []);
				await smtp.Send(model.Email, templateFailed.Subject, templateFailed.Body);
				throw new BadRequestException(ResponseConstants.AUTH_USER_OR_PASSWORD_NOT_FOUND);
			}

			var token = TokenHelper.Create(collaborator.Id, [.. collaborator.CollaboratorRoleCollaborators.Select(x => x.Role.Name)], configuration, cacheService);
			var refreshToken = TokenHelper.CreateRefresh(collaborator.Id, configuration, cacheService);

			var templateSuccess = await emailTemplateService.Get(EmailTemplateNameConstants.AUTH_LOGIN_SUCCESS, new Dictionary<string, string>
			{
				{ "datetime", DateTimeHelper.UtcNow().ToString() }
			});
			await smtp.Send(model.Email, templateSuccess.Subject, templateSuccess.Body);

			return ResponseHelper.Create(new LoginAuthResponse
			{
				Token = token,
				RefreshToken = refreshToken
			});
		}

		public async Task<GenericResponse<LoginAuthResponse>> Renew(RenewAuthRequest model)
		{
			var findRefreshToken = cacheService.Get<RefreshToken>(CacheHelper.AuthRefreshTokenKey(model.RefreshToken))
				?? throw new NotFoundException(ResponseConstants.AUTH_REFRESH_TOKEN_NOT_FOUND);

			var collaborator = await uow.collaboratorRepository.Get(findRefreshToken.CollaboratorId)
				?? throw new NotFoundException(ResponseConstants.COLLABORATOR_NOT_EXISTS);

			var token = TokenHelper.Create(findRefreshToken.CollaboratorId, [.. collaborator.CollaboratorRoleCollaborators.Select(x => x.Role.Name)], configuration, cacheService);
			var refreshToken = TokenHelper.CreateRefresh(findRefreshToken.CollaboratorId, configuration, cacheService);

			cacheService.Delete(CacheHelper.AuthRefreshTokenKey(model.RefreshToken));

			return ResponseHelper.Create(new LoginAuthResponse
			{
				Token = token,
				RefreshToken = refreshToken
			});
		}

		public async Task<GenericResponse<string>> RegisterInit(RegisterInitAuthRequest model)
		{
			if (await uow.collaboratorRepository.IfExists((x => x.Email == model.Email)))
			{
				throw new BadRequestException("El correo electrónico que ingresó, está registrado en la plataforma");
			}

			var token = Generate.RandomText();
			var cacheKey = CacheHelper.AuthRegisterTokenCreation(token, TimeSpan.FromMinutes(5));

			var url = $"{configuration[ConfigurationConstants.CLIENT_ORIGIN]}/register/{token}";
			var template = await emailTemplateService.Get(EmailTemplateNameConstants.AUTH_REGISTER_EMAIL_VERIFICATION, new Dictionary<string, string>
			{
				{ "url", url }
			});

			await smtp.Send(model.Email, template.Subject, template.Body);
			cacheService.Create(cacheKey.Key, cacheKey.Expiration, new RegisterInitAuthResponse
			{
				Email = model.Email
			});

			return ResponseHelper.Create("Enlace para verificación envíado correctamente");
		}

		public async Task<GenericResponse<RegisterInitAuthResponse>> RegisterValidateToken(string token)
		{
			var findToken = cacheService.Get<RegisterInitAuthResponse>(CacheHelper.AuthRegisterTokenKey(token));

			return findToken is null
				? throw new NotFoundException("El token no existe o expiró")
				: ResponseHelper.Create(findToken);
		}

		public async Task<GenericResponse<CollaboratorDto>> RegisterComplete(CreateCollaboratorRequest model, string token)
		{
			var findToken = cacheService.Get<RegisterInitAuthResponse>(CacheHelper.AuthRegisterTokenKey(token));
			if (findToken is null)
			{
				throw new NotFoundException("El token no existe o expiró");
			}

			if (findToken.Email != model.Email)
			{
				throw new BadRequestException("El correo electrónico debe ser el mismo, que se uso al comenzar el proceso de registro.");
			}

			return await collaboratorService.Create(model, null);
		}

		public async Task<GenericResponse<string>> RecoverPasswordSendOTP(RecoverPasswordSendOTPAuthRequest model)
		{
			var collaborator = await uow.collaboratorRepository.Get(x => x.Email == model.Email);

			if (collaborator is not null)
			{
				var otp = Generate.RandomText(length: 8);
				var cacheKey = CacheHelper.AuthRecoverPasswordOTPCreation(otp, TimeSpan.FromMinutes(3));

				var template = await emailTemplateService.Get(EmailTemplateNameConstants.AUTH_RECOVER_PASSWORD_OTP, new Dictionary<string, string>
				{
					{ "otp", otp }
				});
				await smtp.Send(model.Email, template.Subject, template.Body);

				cacheService.Create(cacheKey.Key, cacheKey.Expiration, collaborator.Id);
			}

			return ResponseHelper.Create("Si su correo electrónico existe, recibirá un correo electrónico a su cuenta, con un código que le permitirá realizar su cambio.");
		}

		public async Task<GenericResponse<string>> RecoverPassword(RecoverPasswordAuthRequest model, string code)
		{
			var collaboratorId = cacheService.Get<Guid>(CacheHelper.AuthRecoverPasswordOTPKey(code));

			if (Guid.Empty == collaboratorId)
			{
				throw new NotFoundException("El código que ingresó, es incorrecto o expiró");
			}

			var collaborator = await uow.collaboratorRepository.Get(x => x.Id == collaboratorId)
				?? throw new NotFoundException("Imposible encontrar al colaborador, para completar esta petición");

			collaborator.Password = Hasher.HashPassword(model.NewPassword);
			await uow.collaboratorRepository.Update(collaborator);

			var template = await emailTemplateService.Get(EmailTemplateNameConstants.AUTH_PASSWORD_CHANGED, []);
			await smtp.Send(collaborator.Email, template.Subject, template.Body);

			await uow.SaveChangesAsync();

			return ResponseHelper.Create("Contraseña cambiada con éxito");
		}

		public async Task<GenericResponse<string>> ChangePassword(ChangePasswordAuthRequest model, Claim claim)
		{
			var executor = await collaboratorService.GetExecutor(claim.Value);

			if (!Hasher.ComparePassword(model.CurrentPassword, executor.Password))
			{
				throw new BadRequestException("La contraseña que argumentó como actual, es incorrecta.");
			}

			executor.Password = Hasher.HashPassword(model.NewPassword);

			await uow.collaboratorRepository.Update(executor);

			var template = await emailTemplateService.Get(EmailTemplateNameConstants.AUTH_PASSWORD_CHANGED, []);
			await smtp.Send(executor.Email, template.Subject, template.Body);

			await uow.SaveChangesAsync();

			return ResponseHelper.Create("Contraseña cambiada correctamente");
		}
	}
}
