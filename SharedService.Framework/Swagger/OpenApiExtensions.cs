using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace SharedService.Framework.Swagger;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiSpec(this IServiceCollection services, string serviceName = "Service")
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{serviceName} API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Your company"
                }
            });
        });

        return services;
    }
}