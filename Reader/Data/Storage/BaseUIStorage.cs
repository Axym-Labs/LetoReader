namespace Reader.Data.Storage;

using Reader.Data.Storage;

public class Link
{
    public string Name { get; }
    public string Url { get; }
    public string Target { get; }

    public Link(string name, string url, string target = "_self")
    {
        Name = name;
        Url = url;
        Target = target;
    }
}

public class BaseUIStorage
{
    public class Navbar
    {

        public IReadOnlyList<Link> Links { get; } = new List<Link>
        {
            new Link("Read", "/read"),
            new Link("Roadmap", "https://reader.canny.io/", "_blank"),
            new Link("On GitHub", Constants.GithubRepoUrl, "_blank"),
        };

        public string SpecialAnnouncement { get; } = "";
        public bool SpecialAnnouncementBypass { get; } = false;
    }

    public class Footer
    {

        public IReadOnlyDictionary<string, IReadOnlyList<Link>> Links { get; } = new Dictionary<string, IReadOnlyList<Link>>
        {
            ["Category One"] = new List<Link>
            {
                new Link("Home", "/"),
                new Link("Roadmap", Constants.CannyUrl, "_blank"),
                new Link("Feature requests", Constants.CannyUrl + "/feature-requests", "_blank"),
                new Link("Repository", Constants.GithubRepoUrl, "_blank"),
                new Link("Report Issue", Constants.GithubRepoUrl + "/issues/new", "_blank"),
                new Link("Contact", "mailto:axym@davidewiest.com"),
                new Link("Axym", Constants.GithubOrganizationurl, "_blank"),
            }
        };
    }

    public class Subfooter
    {
        public class Link
        {
            public string Name { get; }
            public string Url { get; }

            public Link(string name, string url)
            {
                Name = name;
                Url = url;
            }
        }

        public IReadOnlyList<Link> Links { get; } = new List<Link>
        {
            new Link("Terms of Service", "/terms-of-service"),
            new Link("Privacy Policy", "/impressum-privacy-policy#privacy-policy"),
            new Link("Impressum", "/impressum-privacy-policy#impressum")
        };
    }

    public class SiteBase
    {
        public string ProjectName { get; } = "Reader";
    }

    public Navbar NavbarData { get; } = new Navbar();
    public Footer FooterData { get; } = new Footer();
    public Subfooter SubfooterData { get; } = new Subfooter();
    public SiteBase SiteBaseData { get; } = new SiteBase();
}
