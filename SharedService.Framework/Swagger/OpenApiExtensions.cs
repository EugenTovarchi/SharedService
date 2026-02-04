using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace SharedService.Framework.Swagger;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiSpec(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DirectoryService",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Yudjine"
                }
            });
        });

        return services;
    }
}