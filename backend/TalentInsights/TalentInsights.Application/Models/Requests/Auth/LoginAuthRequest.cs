using System.ComponentModel.DataAnnotations;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Models.Requests.Auth
{
	public class LoginAuthRequest
	{
		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[EmailAddress(ErrorMessage = ValidationConstants.EMAIL_ADDRESS)]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(255, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(3, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		public string Password { get; set; } = null!;
	}
}
