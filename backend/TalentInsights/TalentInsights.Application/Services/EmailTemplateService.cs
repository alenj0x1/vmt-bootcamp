using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Services.EmailTemplates;
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Application.Services
{
	public class EmailTemplateService(EmailTemplateData data, IEmailTemplateRepository repository) : IEmailTemplateService
	{
		public async Task<EmailTemplateDto> Get(string name, Dictionary<string, string> variables)
		{
			var template = data.Data.First(x => x.Name == name);

			foreach (var variable in variables)
			{
				template.Body = template.Body.Replace("{{" + variable.Key + "}}", variable.Value);
			}

			return new EmailTemplateDto
			{
				Body = template.Body,
				Subject = template.Subject,
			};
		}

		public async Task Init()
		{
			var templates = await repository.Get();
			data.Data = templates;
		}
	}
}
