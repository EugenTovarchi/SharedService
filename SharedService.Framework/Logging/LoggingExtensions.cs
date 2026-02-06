using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;

namespace SharedService.Framework.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName = "Service")
    {
        if (Log.Logger == null || !(Log.Logger is Logger))
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ServiceName", serviceName)
                .CreateLogger();
        }

        services.AddSerilog();

        return services;
    }
}