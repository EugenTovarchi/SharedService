using Microsoft.AspNetCore.Routing;

namespace SharedService.Framework.EndpointSettings;

public interface IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app);
}