namespace Reader.Data.Product;

public class FeedItem
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }
    public DateTime? PubDate { get; private set; }

    public FeedItem(string title, string description, string link, DateTime? pubDate)
    {
        Title = title;
        Description = description;
        Link = link;
        PubDate = pubDate;
    }
}