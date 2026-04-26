using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;

namespace SharedService.Framework.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName = "Service")
    {
        if (Log.Logger == null || Log.Logger is not Logger)
        {
            string? lokiUrl = configuration["Logging:Loki:Url"];

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ServiceName", serviceName);

            if (!string.IsNullOrWhiteSpace(lokiUrl))
            {
                loggerConfiguration.WriteTo.GrafanaLoki(
                    lokiUrl,
                    labels: new[] { new LokiLabel { Key = "service", Value = serviceName } });
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        services.AddSerilog();

        return services;
    }
}