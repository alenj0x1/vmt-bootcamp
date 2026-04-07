using Microsoft.AspNetCore.Authentication.JwtBearer;
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
		public async static Task AddCore(this IServiceCollection services, IConfiguration configuration)
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

			var databaseConnectionString = Environment.GetEnvironmentVariable(ConfigurationConstants.CONNECTION_STRING_DATABASE)
					?? configuration[ConfigurationConstants.CONNECTION_STRING_DATABASE];

			services.AddSqlServer<TalentInsightsContext>(databaseConnectionString);
			services.AddRepositories();

			services.AddServices();

			services.AddMiddlewares();

			AddLogging(services);

			await Initialize(services);
		}

		/// <summary>
		/// Método que añade los middlewares de la aplicación
		/// </summary>
		/// <param name="services"></param>
		public static void AddMiddlewares(this IServiceCollection services)
		{
			services.AddScoped<ErrorHandlerMiddleware>();
		}

		/// <summary>
		/// Método para añadir todo lo relacionado al logging
		/// </summary>
		/// <param name="services"></param>
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

		public async static Task Initialize(this IServiceCollection services)
		{
			var provider = services.BuildServiceProvider();
			var scope = provider.CreateAsyncScope();

			var collaboratorService = scope.ServiceProvider.GetRequiredService<ICollaboratorService>();
			await collaboratorService.CreateFirstUser();
		}

		public static void AddAuth(IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(builder =>
			{
				builder.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				builder.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(builder =>
			{
				var issuer = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_ISSUER)
					?? configuration[ConfigurationConstants.JWT_ISSUER]
					?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_ISSUER));

				var audience = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_AUDIENCE)
					?? configuration[ConfigurationConstants.JWT_AUDIENCE]
					?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_AUDIENCE));

				var privateKey = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_PRIVATE_KEY)
					?? configuration[ConfigurationConstants.JWT_PRIVATE_KEY]
					?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_PRIVATE_KEY));

				var expirationInMinutes = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_PRIVATE_KEY)
					?? configuration[ConfigurationConstants.JWT_PRIVATE_KEY]
					?? "10";

				builder.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = "http://localhost:5063"
				};
			});
		}
	}
}
