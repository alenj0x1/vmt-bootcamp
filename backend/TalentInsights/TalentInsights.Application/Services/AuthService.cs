using Microsoft.Extensions.Configuration;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.Helpers;
using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;
using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Shared;
using TalentInsights.Shared.Constants;
using TalentInsights.Shared.Helpers;

namespace TalentInsights.Application.Services
{
	public class AuthService(IUnitOfWork uow, IConfiguration configuration, ICacheService cacheService, IEmailTemplateService emailTemplateService, SMTP smtp) : IAuthService
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
	}
}
