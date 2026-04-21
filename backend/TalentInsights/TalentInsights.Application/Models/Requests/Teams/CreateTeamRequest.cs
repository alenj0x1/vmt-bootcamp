using System.ComponentModel.DataAnnotations;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Models.Requests.Teams
{
	public class CreateTeamRequest
	{
		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(150, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(500, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		public string? Description { get; set; }
	}
}
