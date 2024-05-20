using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Xml.Linq;

namespace Reader.Modules.Product;

using Reader.Data.ProductExceptions;
using Reader.Data.Product;
using Reader.Modules.Logging;

public static class RssFeedParser
{
    public static List<FeedItem> Parse(string feedUrl)
    {
        try
        {
            // Download the feed content
            string content = DownloadFeedContent(feedUrl);

            // Parse the XML content using XDocument
            XDocument doc = XDocument.Parse(content);

            // Extract channel elements (may vary based on specific RSS format)
            var channel = doc.Descendants("channel").FirstOrDefault();
            if (channel == null)
            {
                throw new Exception("Invalid RSS feed format. Channel element not found.");
            }

            // Extract feed items
            var items = channel.Descendants("item").Select(item =>
            {
                var title = item.Element("title")?.Value;
                var description = item.Element("description")?.Value;
                var link = item.Element("link")?.Value;
                var pubDate = item.Element("pubDate")?.Value;

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(link) || string.IsNullOrEmpty(pubDate))
                {
                    
                }

                DateTime? date = null;
                if (!string.IsNullOrEmpty(pubDate))
                {
                    try
                    {
                        string parseFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";
                        date = DateTime.ParseExact(pubDate, parseFormat, CultureInfo.InvariantCulture);
                    } catch (FormatException e)
                    {
                        Log.Error("Format exception in RSS feed");
                    }
                }


                return new FeedItem(title ?? string.Empty, description ?? string.Empty, link ?? string.Empty, date);
            });

            return items.ToList();
        }
        catch (Exception ex)
        {
            // Handle parsing exceptions
            Console.WriteLine($"Error parsing RSS feed: {ex.Message}");
            return new List<FeedItem>();
        }
    }

    // Helper method to download feed content (implementation omitted for brevity)
    private static string DownloadFeedContent(string url)
    {
        // Implement logic to download the content from the URL (e.g., using WebClient)
        throw new NotImplementedException("DownloadFeedContent method not implemented.");
    }
}