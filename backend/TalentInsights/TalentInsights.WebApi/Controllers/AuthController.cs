using Microsoft.AspNetCore.Mvc;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.Requests.Auth;

namespace TalentInsights.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(IAuthService service) : ControllerBase
	{
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginAuthRequest model)
		{
			var srv = await service.Login(model);
			return Ok(srv);
		}
	}
}
