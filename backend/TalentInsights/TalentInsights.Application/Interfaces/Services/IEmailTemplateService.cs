using TalentInsights.Application.Models.DTOs;

namespace TalentInsights.Application.Interfaces.Services
{
	public interface IEmailTemplateService
	{
		Task<EmailTemplateDto> Get(string name, Dictionary<string, string> variables);
		Task Init();
	}
}
