using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TalentInsights.Application.Helpers;
using TalentInsights.Application.Interfaces.Services;
using TalentInsights.Application.Models.Services.EmailTemplates;
using TalentInsights.Application.Services;
using TalentInsights.Domain.Database.SqlServer;
using TalentInsights.Domain.Database.SqlServer.Context;
using TalentInsights.Domain.Exceptions;
using TalentInsights.Domain.Interfaces.Repositories;
using TalentInsights.Infrastructure.Persistence.SqlServer;
using TalentInsights.Infrastructure.Persistence.SqlServer.Repositories;
using TalentInsights.Shared;
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
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ICacheService, CacheService>();
			services.AddScoped<IEmailTemplateService, EmailTemplateService>();
			services.AddScoped<IAppService, AppService>();
			services.AddScoped<IProjectService, ProjectService>();
			services.AddScoped<ITeamService, TeamService>();
		}

		/// <summary>
		/// Método que sirve para añadir todos los repositorios de la aplicación
		/// </summary>
		/// <param name="services"></param>
		public static void AddRepositories(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();
			services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
			services.AddScoped<IRoleRepository, RoleRepository>();
			services.AddScoped<IProjectRepository, ProjectRepository>();
			services.AddScoped<ITeamRepository, TeamRepository>();
		}

		public async static Task AddSMTP(this IServiceCollection services, IConfiguration configuration)
		{
			// Environment.GetEnvironmentVariable - Para producción
			// configuration[ConfigurationConstants.SMTP_HOST] - Para desarrollo
			var host = Environment.GetEnvironmentVariable(EnvironmentConstants.SMTP_HOST)
				?? configuration[ConfigurationConstants.SMTP_HOST]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.SMTP_HOST));

			var from = Environment.GetEnvironmentVariable(EnvironmentConstants.SMTP_FROM)
				?? configuration[ConfigurationConstants.SMTP_FROM]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.SMTP_FROM));

			var portValue = Environment.GetEnvironmentVariable(EnvironmentConstants.SMTP_PORT) ??
				configuration[ConfigurationConstants.SMTP_PORT];

			var port = Convert.ToInt32(portValue ?? "587");

			var user = Environment.GetEnvironmentVariable(EnvironmentConstants.SMTP_USER)
				?? configuration[ConfigurationConstants.SMTP_USER]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.SMTP_USER));

			var password = Environment.GetEnvironmentVariable(EnvironmentConstants.SMTP_PASSWORD)
				?? configuration[ConfigurationConstants.SMTP_PASSWORD]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.SMTP_PASSWORD));

			var smtp = new SMTP(host, from, port, user, password);
			services.AddSingleton(smtp);
		}


		/// <summary>
		/// Método que añade lo esencial que necesita nuestra aplicación para funcionar
		/// </summary>
		/// <param name="services"></param>
		public async static Task AddCore(this IServiceCollection services, IConfiguration configuration)
		{
			await services.AddSMTP(configuration);

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

			var databaseConnectionString = Environment.GetEnvironmentVariable(EnvironmentConstants.CONNECTION_STRING_DATABASE)
					?? configuration[ConfigurationConstants.CONNECTION_STRING_DATABASE];

			services.AddSqlServer<TalentInsightsContext>(databaseConnectionString);
			services.AddRepositories();

			services.AddServices();

			services.AddMiddlewares();

			services.AddLogging();

			services.AddAuth(configuration);

			services.AddCache();

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
			var templatesData = new EmailTemplateData();
			services.AddSingleton(templatesData);

			var provider = services.BuildServiceProvider();
			var scope = provider.CreateAsyncScope();

			var collaboratorService = scope.ServiceProvider.GetRequiredService<ICollaboratorService>();
			await collaboratorService.CreateFirstUser();

			var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
			await emailTemplateService.Init();
		}

		public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(builder =>
			{
				builder.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				builder.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(builder =>
			{
				var tokenConfiguration = TokenHelper.Configuration(configuration);

				builder.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = tokenConfiguration.Issuer,
					ValidateAudience = true,
					ValidAudience = tokenConfiguration.Audience,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = tokenConfiguration.SecurityKey,
					ClockSkew = TimeSpan.Zero
				};

				builder.Events = new JwtBearerEvents
				{
					OnChallenge = async context =>
					{
						throw new UnauthorizedException(ResponseConstants.AUTH_TOKEN_NOT_FOUND);
					}
				};
			});

			services.AddAuthorization();
		}

		public static void AddCache(this IServiceCollection services)
		{
			services.AddMemoryCache();
		}
	}
}
