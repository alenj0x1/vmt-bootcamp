using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface IAppService
	{
		Task<GenericResponse<AppInfoDto>> Info();
	}
}
