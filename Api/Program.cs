using Microsoft.Extensions.Diagnostics.HealthChecks;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

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

WebApplication app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapHealthChecks("/health");

app.Run();