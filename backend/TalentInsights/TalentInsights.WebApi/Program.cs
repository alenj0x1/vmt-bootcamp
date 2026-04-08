using Scalar.AspNetCore;
using Serilog;
using TalentInsights.WebApi.Extensions;
using TalentInsights.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
await builder.Services.AddCore(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapScalarApiReference(options =>
	{
		options.Theme = ScalarTheme.Mars;
		options.WithTitle("Talent Insights");
	});
	app.MapOpenApi();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
