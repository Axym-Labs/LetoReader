namespace Reader.Modules.Logging;

using Serilog;
using Serilog.Events;
using Reader.Data.Storage;
using System.Text;

class BasicLogEvent
{
    public string MessageTemplate { get; set; }
    public object[] PropertyValues { get; set; }

    public BasicLogEvent(string messageTemplate, object[] propertyValues)
    {
        MessageTemplate = messageTemplate;
        PropertyValues = propertyValues;
    }
}

public static class Log
{
    private static readonly ILogger SLogger = new LoggerConfiguration()
    #if DEBUG
    .MinimumLevel.Debug()
    #endif
    .Enrich.With(new LoggingOptionEnricher())
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning || evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
        .WriteTo.File("Logs/priority_.txt", rollingInterval: RollingInterval.Day)
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
        .WriteTo.File("Logs/error_.txt", rollingInterval: RollingInterval.Day)
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Properties.ContainsKey("Channel") && evt.Properties["Channel"].ToString() == "Data")
        .WriteTo.File("Logs/data_.txt", rollingInterval: RollingInterval.Day)
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning || evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
        .WriteTo.Console()
    )
    .WriteTo.File("Logs/log_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

    public static void Verbose(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Verbose(messageTemplate, propertyValues);
    }

    public static void Debug(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Debug(messageTemplate, propertyValues);
    }

    public static async Task Information(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Information(messageTemplate, propertyValues);
        await PostToCentralLog(messageTemplate, propertyValues);
    }

    public static async Task Warning(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Warning(messageTemplate, propertyValues);
        await PostToCentralLog(messageTemplate, propertyValues);
    }

    public static async Task Error(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Error(messageTemplate, propertyValues);
        await PostToCentralLog(messageTemplate, propertyValues);
    }

    public static async Task Fatal(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Fatal(messageTemplate, propertyValues);
        await PostToCentralLog(messageTemplate, propertyValues);

    }

    public static void Data(string messageTemplate, params object[] propertyValues)
    {
        var properties = new Dictionary<string, object> { ["Channel"] = "Data" };
        SLogger.Information(messageTemplate, propertyValues.Concat(new object[] { properties }).ToArray());
    }

    private static async Task PostToCentralLog(string messageTemplate, params object[] propertyValues)
    {
        var logEvent = new BasicLogEvent(messageTemplate, propertyValues);
        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(logEvent);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(4);
                _ = Task.Run(async () => await httpClient.PostAsync(Constants.CentralLoggerEndpoint, content));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("cant post to central logger endpoint: " + e.ToString());
        }
    }
}
