using Discord;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Shared.Data;
using Shared.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(logging => {
	logging.IncludeFormattedMessage = true;
	logging.IncludeScopes           = true;
});

builder.Services.AddOpenTelemetry()
	   .WithMetrics(metrics => {
			metrics.AddAspNetCoreInstrumentation()
				   .AddHttpClientInstrumentation()
				   .AddRuntimeInstrumentation();
		})
	   .WithTracing(tracing => {
			tracing.AddSource(builder.Environment.ApplicationName)
				   .AddAspNetCoreInstrumentation()
				   .AddHttpClientInstrumentation();
		});

bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

if (useOtlpExporter) builder.Services.AddOpenTelemetry().UseOtlpExporter();

builder.Services.AddServiceDiscovery();

builder.Services.ConfigureHttpClientDefaults(http => {
	http.AddStandardResilienceHandler();

	http.AddServiceDiscovery();
});

builder.Services.AddHealthChecks()
	   .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);


builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<AppDbContext>("db", null, options => {
	options.EnableDetailedErrors();
	options.EnableSensitiveDataLogging();
});

builder.AddRedisDistributedCache("cache");

DiscordExtensions.ConfigureService(builder);
SuperviveService.ConfigureService(builder.Services);
DataIntegrationService.ConfigureService(builder.Services);

WebApplication app = builder.Build();

DiscordExtensions.ConfigureHost(app);

app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapHealthChecks("/health");


await using (AsyncServiceScope scope = app.Services.CreateAsyncScope()) {
	AppDbContext       ctx      = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	IExecutionStrategy strategy = ctx.Database.CreateExecutionStrategy();

	await strategy.ExecuteAsync(async () => {
		await ctx.Database.MigrateAsync();
	});
}

app.Run();