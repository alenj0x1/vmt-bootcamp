namespace TalentInsights.Application.Models.DTOs
{
	public class TeamDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public bool IsPublic { get; set; }
		public List<CollaboratorDto> Members { get; set; } = [];

		public CollaboratorDto CreatedBy { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}
