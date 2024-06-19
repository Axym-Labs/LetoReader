namespace Reader.Modules.Middleware;

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Reader.Modules.Logging;

public class LoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        } catch (System.Exception e)
        {
            await Log.Error("Error in request", e);
            throw;
        }

        stopwatch.Stop();

        var requestUrl = context.Request.Path;
        var requesterIp = context.Connection.RemoteIpAddress;
        var timeTaken = stopwatch.Elapsed.TotalMilliseconds;

        string addressBackup = "::::";

        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            addressBackup = context.Request.Headers["X-Forwarded-For"]!;
        }

        if (requesterIp != null)
        {
            Log.Data("Request Ip={Ip} Url={Url} Time={Time:000.000}", requesterIp.ToString() ?? addressBackup, requestUrl, timeTaken);
        }
    }
}