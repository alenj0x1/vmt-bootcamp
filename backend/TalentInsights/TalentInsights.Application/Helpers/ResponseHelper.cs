using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Helpers
{
	public static class ResponseHelper
	{
		public static GenericResponse<T> Create<T>(T data, List<string>? errors = null, string? message = null)
		{
			var response = new GenericResponse<T>
			{
				Data = data,
				Message = message ?? "Solicitud realizada correctamente",
				Errors = errors ?? []
			};

			return response;
		}
	}
}
