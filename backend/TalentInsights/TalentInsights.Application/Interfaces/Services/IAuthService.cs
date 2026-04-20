using System.Security.Claims;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Requests.Auth.RecoverPassword;
using TalentInsights.Application.Models.Requests.Auth.Register;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface IAuthService
	{
		Task<GenericResponse<LoginAuthResponse>> Login(LoginAuthRequest model);
		Task<GenericResponse<LoginAuthResponse>> Renew(RenewAuthRequest model);

		// Register
		Task<GenericResponse<string>> RegisterInit(RegisterInitAuthRequest model);
		Task<GenericResponse<RegisterInitAuthResponse>> RegisterValidateToken(string token);
		Task<GenericResponse<CollaboratorDto>> RegisterComplete(CreateCollaboratorRequest model, string token);

		// Recover password
		Task<GenericResponse<string>> RecoverPasswordSendOTP(RecoverPasswordSendOTPAuthRequest model);
		Task<GenericResponse<string>> RecoverPassword(RecoverPasswordAuthRequest model, string code);
		Task<GenericResponse<string>> ChangePassword(ChangePasswordAuthRequest model, Claim claim);
	}
}
