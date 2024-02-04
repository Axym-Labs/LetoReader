namespace Reader.Modules.Logging;

using Serilog.Core;
using Serilog.Events;


public class LoggingOptionEnricher : ILogEventEnricher
{
    private readonly bool _NotifyUser;

    public LoggingOptionEnricher(bool NotifyUser = false)
    {
        _NotifyUser = NotifyUser;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var NotifyUserProperty = new LogEventProperty("NotifyUser", new ScalarValue(_NotifyUser));
        logEvent.AddPropertyIfAbsent(NotifyUserProperty);
    }
}