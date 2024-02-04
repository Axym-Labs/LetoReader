namespace Reader.Modules.Logging;

using Serilog;
using Serilog.Events;

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
    .WriteTo.Sink(new LoggingEventHandler())
    .WriteTo.Console()
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

    public static void Information(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Information(messageTemplate, propertyValues);
    }

    public static void Warning(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Warning(messageTemplate, propertyValues);
    }

    public static void Error(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Error(messageTemplate, propertyValues);
    }

    public static void Fatal(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Fatal(messageTemplate, propertyValues);
    }

    public static void Data(string messageTemplate, params object[] propertyValues)
    {
        var properties = new Dictionary<string, object> { ["Channel"] = "Data" };
        SLogger.Information(messageTemplate, propertyValues.Concat(new object[] { properties }).ToArray());
    }
}
