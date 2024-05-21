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

public class BaseUIStorage
{
    public class Navbar
    {

        public IReadOnlyList<Link> Links { get; } = new List<Link>
        {
            new Link("Read", "/read"),
            new Link("Roadmap", "https://reader.canny.io/", "_blank"),
            new Link("On GitHub", "https://github.com/Axym-Labs/Leto-Reader", "_blank"),
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
                new Link("Roadmap", "https://reader.canny.io/", "_blank"),
                new Link("Feature requests", "https://reader.canny.io/feature-requests", "_blank"),
                new Link("Repository", "https://github.com/Axym-Labs/Leto-Reader", "_blank"),
                new Link("Report Issue", "https://github.com/Axym-Labs/Leto-Reader/issues/new", "_blank"),
                new Link("Contact", "mailto:axym@davidewiest.com"),
                new Link("Axym", "https://github.com/Axym-Labs", "_blank"),
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
