using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SharedService.Framework.EndpointSettings;

public static class EndpointExtensions
{
    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}