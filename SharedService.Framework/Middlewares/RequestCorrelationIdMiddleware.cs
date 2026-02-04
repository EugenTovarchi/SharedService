using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace SharedService.Framework.Middlewares;

/// <summary>
/// Отслеживаем логи запроса по этому id в Seq (CorrelationId="0GHJQKRj000f:0000231")
/// По сути тоже самое что и RequestId.
/// </summary>
public class RequestCorrelationIdMiddleware(RequestDelegate next)
{
    private const string CORRELATION_ID_HEADER_NAME = "X-Correlation-Id";

    private const string CORRELATION_ID = "Correlation-Id";

    public Task Invoke(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue(CORRELATION_ID_HEADER_NAME, out StringValues correlationIdValues);

        string correlationId = correlationIdValues.FirstOrDefault() ?? httpContext.TraceIdentifier;

        using (LogContext.PushProperty(CORRELATION_ID, correlationId))
        {
            return next(httpContext);
        }
    }
}

public static class RequestCorrelationIdMiddlewareExtension
{
    public static IApplicationBuilder UseRequestCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestCorrelationIdMiddleware>();
    }
}