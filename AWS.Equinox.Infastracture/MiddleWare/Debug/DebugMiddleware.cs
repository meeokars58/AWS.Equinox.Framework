using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace AWS.Equinox.Infastracture.MiddleWare.Debug;

public class DebugMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DebugMiddleware> _logger;

    public DebugMiddleware(RequestDelegate next, ILogger<DebugMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        var elapsedTime = stopwatch.Elapsed;
    }
}