namespace Reader.Data.Storage;

public class Constants
{
    public const string ProjectName = "Axym";
    public const string ProjectDir = "Reader";
    public const string ContentDir = "/static/content";
    public int CurrentYear = DateTime.Now.Year;

    public const string CentralLoggerEndpoint = "http://127.0.0.1:5000/log"; //"https://exym-log.davidewiest.com/log";
    public const string CentralAnnouncementsEndpoint = "http://127.0.0.1:5000/announcements.json";
}
