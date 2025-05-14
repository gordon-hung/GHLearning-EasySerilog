using System.Net.Mime;
using System.Text.Json.Serialization;
using CorrelationId;
using CorrelationId.DependencyInjection;
using GHLearning.EasySerilog.AppOpenTelemetry.Correlations;
using GHLearning.EasySerilog.AppOpenTelemetry.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add<HandleCorrelationActionFilter>();
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Learn more about configuring CorrelationId at https://github.com/stevejgordon/CorrelationId/wiki
builder.Services.AddCorrelationId<CustomCorrelationIdProvider>(options =>
{
	options.AddToLoggingScope = true;
	options.LoggingScopeKey = CorrelationIdOptions.DefaultHeader;
});

// Learn more about configuring  Serilog at https://github.com/serilog/serilog/wiki/Configuration-Basics
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.WithExceptionDetails()
	.Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
	.Enrich.WithProperty("EnvironmentName", builder.Environment.EnvironmentName)
	.Enrich.WithProperty("RuntimeId", SequentialGuid.SequentialGuidGenerator.Instance.NewGuid())
	.Enrich.WithProperty("ApplicationStartAt", DateTimeOffset.UtcNow.ToString("u"))
	.Enrich.WithCorrelationIdHeader(CorrelationIdOptions.DefaultHeader)
	.MinimumLevel.Debug()
	.CreateLogger();

builder.Logging.ClearProviders().AddSerilog();

builder.Host.UseSerilog();

//Learn more about configuring HealthChecks at https://learn.microsoft.com/zh-tw/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-9.0
builder.Services.AddHealthChecks()
	.AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));// swagger/
	app.UseReDoc(options => options.SpecUrl("/openapi/v1.json"));//api-docs/
	app.MapScalarApiReference();//scalar/v1
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
	Predicate = check => check.Tags.Contains("live"),
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
});
app.UseHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
	Predicate = _ => true,
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
});

app.Run();
