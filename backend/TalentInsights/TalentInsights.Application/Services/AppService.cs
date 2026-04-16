using Microsoft.Extensions.Configuration;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Services
{
	public class AppService(IConfiguration configuration) : IAppService
	{
		public async Task<GenericResponse<AppInfoDto>> Info()
		{
			return ResponseHelper.Create(new AppInfoDto
			{
				Version = configuration[ConfigurationConstants.VERSION] ?? "0.0.0"
			});
		}
	}
}
