namespace Reader.Data.Storage;

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

public class FundamentalStorage
{
    public class Navbar
    {

        public IReadOnlyList<Link> Links { get; } = new List<Link>
        {
            new Link("Read", "/read"),
            new Link("Roadmap", "https://reader.canny.io/", "_blank"),
            new Link("Feature Request", "https://reader.canny.io/feature-requests", "_blank"),
            new Link("Repository", "https://github.com/DavideWiest/Reader", "_blank"),
            new Link("Contact", "mailto:contact@timewise.davidewiest.com"),
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
                new Link("Contact", "mailto:hi@davidewiest.com"),
                new Link("Roadmap", "https://reader.canny.io/", "_blank"),
                new Link("Feature requests", "https://reader.canny.io/feature-requests", "_blank"),
                new Link("Repository", "https://github.com/DavideWiest/Reader", "_blank"),
                new Link("Report Issue", "https://github.com/DavideWiest/Reader/issues/new", "_blank"),
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
