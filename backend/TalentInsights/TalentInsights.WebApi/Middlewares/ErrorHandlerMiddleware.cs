using TalentInsights.Domain.Exceptions;

namespace TalentInsights.WebApi.Middlewares
{
	public class ErrorHandlerMiddleware : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch (NotFoundException exception)
			{

				throw;
			}
		}
	}
}
