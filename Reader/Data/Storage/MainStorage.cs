namespace Reader.Data.Storage;

public class MainStorage
{
    public class Hero
    {
        // You can add any properties specific to the Hero section here
    }

    public class AsSeenIn
    {
        public class Source
        {
            public string Name { get; }
            public string ImageUrl { get; }

            public Source(string name, string imageUrl)
            {
                Name = name;
                ImageUrl = imageUrl;
            }
        }

        public IReadOnlyList<Source> Sources { get; } = new List<Source>
        {
            new Source("Tiktok", "tiktok.png"),
            new Source("New York Times", "nyt.png")
        };
    }

    public class BottomCta
    {
        public class CtaLink
        {
            public string Url { get; }
            public string Name { get; }

            public CtaLink(string url, string name)
            {
                Url = url;
                Name = name;
            }
        }

        public IReadOnlyList<CtaLink> Links { get; } = new List<CtaLink>
        {
            new CtaLink("/link", "name"),
            new CtaLink("/agb", "agb")
        };
    }

    public Hero HeroData { get; } = new Hero();
    public AsSeenIn AsSeenInData { get; } = new AsSeenIn();
    public BottomCta BottomCtaData { get; } = new BottomCta();
}
