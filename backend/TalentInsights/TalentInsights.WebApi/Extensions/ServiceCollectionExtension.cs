using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Services;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Interfaces.Repositories;
using TalentInsights.Infrastructure.Persistence.SqlServer.Repositories;
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
			services.AddControllers();
			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
			services.AddOpenApi();

			services.AddSqlServer<TalentInsightsContext>(configuration.GetConnectionString("Database"));
			services.AddRepositories();

			services.AddServices();

			services.AddMiddlewares();
		}

		/// <summary>
		/// Método que añade los middlewares de la aplicación
		/// </summary>
		/// <param name="services"></param>
		public static void AddMiddlewares(this IServiceCollection services)
		{
			services.AddScoped<ErrorHandlerMiddleware>();
		}
	}
}
