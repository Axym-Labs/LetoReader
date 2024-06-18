using Serilog.Events;
using Serilog.Core;
using System.Text;

namespace Reader.Modules.Logging;

public class LoggingEventHandler : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Level == LogEventLevel.Debug || logEvent.Level == LogEventLevel.Verbose)
        {
            return;
        }

        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(logEvent);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        try {
            using (var httpClient = new HttpClient())
            {
                _ = httpClient.PostAsync(Data.Storage.Constants.CentralLoggerEndpoint, content).Result;
            }
        }
        catch (Exception e) {
            Console.WriteLine("cant post to central logger endpoint: " + e.ToString());
        }
    }
}
