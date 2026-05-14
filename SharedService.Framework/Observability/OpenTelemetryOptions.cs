using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;

namespace SharedService.Framework.Observability;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public bool Enabled { get; init; }

    public string ServiceName { get; init; } = string.Empty;

    public string ServiceVersion { get; init; } = "0.0.1";

    public string Environment { get; init; } = string.Empty;

    public OtlpOptions Otlp { get; init; } = new();

    public TelemetrySignalOptions Metrics { get; init; } = new();

    public TelemetrySignalOptions Tracing { get; init; } = new();

    public HealthCheckTelemetryOptions HealthChecks { get; init; } = new();

    internal static OpenTelemetryOptions FromConfiguration(
        IConfiguration configuration,
        string fallbackServiceName)
    {
        IConfigurationSection section = configuration.GetSection(SectionName);

        string serviceName = GetValue(section, nameof(ServiceName), fallbackServiceName);
        string environment = GetValue(section, nameof(Environment), string.Empty);
        string serviceVersion = GetValue(section, nameof(ServiceVersion), "0.0.1");
        string endpoint = GetValue(section.GetSection(nameof(Otlp)), nameof(OtlpOptions.Endpoint), string.Empty);
        string protocol = GetValue(section.GetSection(nameof(Otlp)), nameof(OtlpOptions.Protocol), "Grpc");

        return new OpenTelemetryOptions
        {
            Enabled = GetBool(section, nameof(Enabled), false),
            ServiceName = string.IsNullOrWhiteSpace(serviceName) ? fallbackServiceName : serviceName,
            ServiceVersion = string.IsNullOrWhiteSpace(serviceVersion) ? "0.0.1" : serviceVersion,
            Environment = environment,
            Otlp = new OtlpOptions
            {
                Endpoint = endpoint,
                Protocol = ParseProtocol(protocol),
            },
            Metrics = new TelemetrySignalOptions
            {
                Enabled = GetBool(section.GetSection(nameof(Metrics)), nameof(TelemetrySignalOptions.Enabled), true),
            },
            Tracing = new TelemetrySignalOptions
            {
                Enabled = GetBool(section.GetSection(nameof(Tracing)), nameof(TelemetrySignalOptions.Enabled), false),
            },
            HealthChecks = new HealthCheckTelemetryOptions
            {
                ExcludeSuccessful = GetBool(
                    section.GetSection(nameof(HealthChecks)),
                    nameof(HealthCheckTelemetryOptions.ExcludeSuccessful),
                    true),
            },
        };
    }

    private static string GetValue(IConfiguration configuration, string key, string defaultValue)
    {
        string? value = configuration[key];

        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    private static bool GetBool(IConfiguration configuration, string key, bool defaultValue)
    {
        string? value = configuration[key];

        return bool.TryParse(value, out bool parsedValue) ? parsedValue : defaultValue;
    }

    private static OtlpExportProtocol ParseProtocol(string value)
    {
        return Enum.TryParse(value, ignoreCase: true, out OtlpExportProtocol protocol)
            ? protocol
            : OtlpExportProtocol.Grpc;
    }
}

public sealed class OtlpOptions
{
    public string Endpoint { get; init; } = string.Empty;

    public OtlpExportProtocol Protocol { get; init; } = OtlpExportProtocol.Grpc;
}

public sealed class TelemetrySignalOptions
{
    public bool Enabled { get; init; } = true;
}

public sealed class HealthCheckTelemetryOptions
{
    public bool ExcludeSuccessful { get; init; } = true;
}
