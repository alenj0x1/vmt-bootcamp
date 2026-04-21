namespace TalentInsights.Application.Models.DTOs
{
	public class MenuDto
	{
		public Guid Id { get; set; }

		public string Code { get; set; } = null!;

		public string Name { get; set; } = null!;

		public string Path { get; set; } = null!;

		public string IconName { get; set; } = null!;

		public Guid? ParentId { get; set; }
	}
}
