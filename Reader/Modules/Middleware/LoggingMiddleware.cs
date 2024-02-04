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

        await next(context);

        stopwatch.Stop();

        var requestUrl = context.Request.Path;
        var requesterIp = context.Connection.RemoteIpAddress;
        var timeTaken = stopwatch.Elapsed.TotalMilliseconds;
        if (requesterIp != null)
        {
            Log.Data("Request Ip={Ip} Url={Url} Time={Time:000.000}", requesterIp, requestUrl, timeTaken);
        } else
        {
            Log.Data("Request Ip=:::: Url={Url} Time={Time:000.000}", requestUrl, timeTaken);
        }
    }
}