using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.Requests.Collaborator;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CollaboratorsController(ICollaboratorService collaboratorService) : ControllerBase
	{
		[HttpPost]
		[Authorize(Roles = "Admin, HR")]
		public async Task<IActionResult> Create([FromBody] CreateCollaboratorRequest model)
		{
			var srv = await collaboratorService.Create(model);
			return Ok(srv);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] FilterColaboratorRequest model, [FromHeader] string authorization)
		{
			var srv = collaboratorService.Get(model);
			return Ok(srv);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var srv = await collaboratorService.Get(id);
			return Ok(srv);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin, HR")]
		public async Task<IActionResult> Update([FromBody] UpdateCollaboratorRequest model, Guid id)
		{
			var srv = await collaboratorService.Update(id, model);
			return Ok(srv);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin, HR")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var srv = await collaboratorService.Delete(id);
			return Ok(srv);
		}
	}
}
