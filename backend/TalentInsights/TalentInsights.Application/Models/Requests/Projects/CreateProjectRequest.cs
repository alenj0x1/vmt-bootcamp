using System.ComponentModel.DataAnnotations;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Models.Requests.Projects
{
	public class CreateProjectRequest
	{
		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(200, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		public string Name { get; set; } = null!;

		[MaxLength(1000, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		public string? Description { get; set; }

		[Required(ErrorMessage = ValidationConstants.REQUIRED)]
		[MaxLength(50, ErrorMessage = ValidationConstants.MAX_LENGTH)]
		[MinLength(3, ErrorMessage = ValidationConstants.MIN_LENGTH)]
		public string Status { get; set; } = null!;
	}
}
