namespace TalentInsights.Application.Models.DTOs
{
	public class ProjectDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string Status { get; set; } = null!;
		public List<TeamDto> Teams { get; set; } = [];

		public CollaboratorDto CreatedBy { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}
