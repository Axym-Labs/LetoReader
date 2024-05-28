namespace Reader.Data.Storage;

public class Constants
{
    public const string ProjectName = "LetoReader";
    public const string ProjectDir = "Reader";

    public const string ShareTitle = "Leto - Free Modern Speed Reading";
    public const string ShareUrl = "https://axym.davidewiest.com";
    public const string ShareDescription = "";

    public const string ContentDir = "/static/content";
    public int CurrentYear = DateTime.Now.Year;

    public const string CentralConnectionUrl = "https://axym-con.davidewiest.com";
    public const string CentralLoggerEndpoint = CentralConnectionUrl + "/log";
    public const string CentralAnnouncementsEndpoint = CentralConnectionUrl + "/announcements.json";
    public const string CentralSpecialAnnouncementEndpoint = CentralConnectionUrl + "/specialAnnouncement.json";

    public const string DockerUrl = "https://hub.docker.com/r/davidewiest/reader";

    public const string GithubOrganizationurl = "https://github.com/Axym-Labs";
    public const string GithubRepoUrl = GithubOrganizationurl + "/LetoReader";
    public const string CannyUrl = "https://reader.canny.io/";
    public const string DocumentationUrl = GithubRepoUrl + "/wiki";
}