using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface IAuthService
	{
		Task<GenericResponse<string>> Login(LoginAuthRequest model);
	}
}
