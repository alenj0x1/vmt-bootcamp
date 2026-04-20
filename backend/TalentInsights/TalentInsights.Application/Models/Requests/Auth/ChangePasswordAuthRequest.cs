using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Models.Requests.Auth
{
	public class ChangePasswordAuthRequest
	{
		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(255, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		[Description("La contraseña actual del usuario")]
		public string CurrentPassword { get; set; } = null!;

		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(255, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		[Description("La contraseña nueva del usuario")]
		public string NewPassword { get; set; } = null!;
	}
}
