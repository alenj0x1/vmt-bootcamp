using TalentInsights.Shared.Helpers;

namespace TalentInsights.Application.Models.Responses
{
	public class GenericResponse<T>
	{
		public string Message { get; set; }
		public List<string> Errors { get; set; } = [];
		public DateTime TimeStamp { get; } = DateTimeHelper.UtcNow();
		public int Count { get; set; } = 0;
		public T Data { get; set; }
	}
}
