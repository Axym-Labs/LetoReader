using Serilog.Events;
using Serilog.Core;
using System.Text;

namespace Reader.Modules.Logging;

public class LoggingEventHandler : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(logEvent);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        using (var httpClient = new HttpClient())
        {
            _ = Task.Run(() => httpClient.PostAsync(Data.Storage.Constants.CentralLoggerEndpoint, content));
        }
    }
}
