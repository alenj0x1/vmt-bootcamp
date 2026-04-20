using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Models.Requests.Auth.RecoverPassword
{
	public class RecoverPasswordSendOTPAuthRequest
	{
		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[EmailAddress(ErrorMessage = ValidationConstants.EMAIL_ADDRESS)]
		[MaxLength(255, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		[Description("El correo electrónico del usuario, para envíarle el código OTP")]
		public string Email { get; set; } = null!;
	}
}
