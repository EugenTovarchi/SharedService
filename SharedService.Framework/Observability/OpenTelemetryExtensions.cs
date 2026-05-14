using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SharedService.Framework.Observability;

public static class OpenTelemetryExtensions
{
    private static readonly string[] HealthCheckPaths =
    [
        "/health",
        "/healthz",
        "/ready",
        "/live",
        "/nginx/health",
    ];

    public static IServiceCollection AddSharedOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string fallbackServiceName = "Service")
    {
        OpenTelemetryOptions options = OpenTelemetryOptions.FromConfiguration(configuration, fallbackServiceName);

        if (!options.Enabled)
        {
            return services;
        }

        var openTelemetryBuilder = services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => ConfigureResource(resourceBuilder, options));

        if (options.Metrics.Enabled)
        {
            openTelemetryBuilder.WithMetrics(metricsBuilder =>
            {
                metricsBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                ConfigureOtlpExporter(metricsBuilder, options);
            });
        }

        if (options.Tracing.Enabled)
        {
            openTelemetryBuilder.WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation(instrumentationOptions =>
                    {
                        // ASP.NET Core filter runs before response status is known, so failed health checks on these paths are filtered too.
                        instrumentationOptions.Filter = context =>
                            ShouldTraceRequest(context, options.HealthChecks.ExcludeSuccessful);
                    })
                    .AddHttpClientInstrumentation();

                ConfigureOtlpExporter(tracerProviderBuilder, options);
            });
        }

        return services;
    }

    private static void ConfigureResource(ResourceBuilder resourceBuilder, OpenTelemetryOptions options)
    {
        resourceBuilder.AddService(
            serviceName: options.ServiceName,
            serviceVersion: options.ServiceVersion,
            autoGenerateServiceInstanceId: true);

        if (!string.IsNullOrWhiteSpace(options.Environment))
        {
            resourceBuilder.AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = options.Environment,
            });
        }
    }

    private static void ConfigureOtlpExporter(MeterProviderBuilder metricsBuilder, OpenTelemetryOptions options)
    {
        metricsBuilder.AddOtlpExporter(exporterOptions =>
        {
            ConfigureOtlpExporter(exporterOptions, options);
        });
    }

    private static void ConfigureOtlpExporter(TracerProviderBuilder tracerProviderBuilder, OpenTelemetryOptions options)
    {
        tracerProviderBuilder.AddOtlpExporter(exporterOptions =>
        {
            ConfigureOtlpExporter(exporterOptions, options);
        });
    }

    private static void ConfigureOtlpExporter(
        OpenTelemetry.Exporter.OtlpExporterOptions exporterOptions,
        OpenTelemetryOptions options)
    {
        if (Uri.TryCreate(options.Otlp.Endpoint, UriKind.Absolute, out Uri? endpoint))
        {
            exporterOptions.Endpoint = endpoint;
        }

        exporterOptions.Protocol = options.Otlp.Protocol;
    }

    private static bool ShouldTraceRequest(HttpContext context, bool excludeSuccessfulHealthChecks)
    {
        if (!excludeSuccessfulHealthChecks)
        {
            return true;
        }

        return !IsHealthCheckPath(context.Request.Path);
    }

    private static bool IsHealthCheckPath(PathString requestPath)
    {
        foreach (string healthCheckPath in HealthCheckPaths)
        {
            if (requestPath.StartsWithSegments(healthCheckPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
