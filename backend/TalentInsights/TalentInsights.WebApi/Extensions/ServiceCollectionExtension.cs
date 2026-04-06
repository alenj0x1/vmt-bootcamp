using Microsoft.AspNetCore.Mvc;
using Serilog;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Services;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Interfaces.Repositories;
using TalentInsights.Infrastructure.Persistence.SqlServer.Repositories;
using TalentInsights.Shared.Constants;
using TalentInsights.WebApi.Middlewares;

namespace TalentInsights.WebApi.Extensions
{
	public static class ServiceCollectionExtension
	{
		/// <summary>
		/// Método que sirve para añadir todos los servicios de la aplicación
		/// </summary>
		/// <param name="services"></param>
		public static void AddServices(this IServiceCollection services)
		{
			services.AddScoped<ICollaboratorService, CollaboratorService>();
		}

		/// <summary>
		/// Método que sirve para añadir todos los repositorios de la aplicación
		/// </summary>
		/// <param name="services"></param>
		public static void AddRepositories(this IServiceCollection services)
		{
			services.AddTransient<ICollaboratorRepository, CollaboratorRepository>();
		}


		/// <summary>
		/// Método que añade lo esencial que necesita nuestra aplicación para funcionar
		/// </summary>
		/// <param name="services"></param>
		public static void AddCore(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllers().ConfigureApiBehaviorOptions(options =>
			{
				options.InvalidModelStateResponseFactory = (errorContext) =>
				{
					var errors = errorContext.ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage).ToList()).ToList();
					var response = ResponseHelper.Create(
						data: ValidationConstants.VALIDATION_MESSAGE,
						errors: errors,
						message: ValidationConstants.VALIDATION_MESSAGE
						);
					return new BadRequestObjectResult(response);
				};
			});
			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
			services.AddOpenApi();

			services.AddSqlServer<TalentInsightsContext>(configuration.GetConnectionString("Database"));
			services.AddRepositories();

			services.AddServices();

			services.AddMiddlewares();

			AddLogging(services);
		}

		/// <summary>
		/// Método que añade los middlewares de la aplicación
		/// </summary>
		/// <param name="services"></param>
		public static void AddMiddlewares(this IServiceCollection services)
		{
			services.AddScoped<ErrorHandlerMiddleware>();
		}

		public static void AddLogging(this IServiceCollection services)
		{
			services.AddSerilog();

			Log.Logger = new LoggerConfiguration()
				// File
				.WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.txt"), rollingInterval: RollingInterval.Day)
				// Console
				.WriteTo.Console()
				.CreateLogger();
		}
	}
}
