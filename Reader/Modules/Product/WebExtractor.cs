using HtmlAgilityPack;

namespace Reader.Modules.Product;

using Reader.Data.ProductExceptions;

public record WebsiteData<T>()
{
    public string Url { get; set; }
    public string Title { get; set; }
    public T Content { get; set; }
}

public class WebExtractor
{
    private readonly HttpClient _httpClient;

    private HtmlDocument doc = new HtmlDocument();

    public WebExtractor()
    {
        _httpClient = new HttpClient();

    }

    public async Task Load(string url)
    {
        var html = await GetHtml(url);
        LoadFromHtml(html);
    }

    public void LoadFromHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            throw new ScrapingException("Empty html document");
        }

        doc.LoadHtml(html);
    }

    public HtmlNode GetNodeByXPath(string xpath)
    {
        return GetNodeByXPathJoined(xpath).First();
    }

    public IEnumerable<HtmlNode> GetAllNodesByXPath(string xpath)
    {
        return GetNodeByXPathJoined(xpath);
    }

    public IEnumerable<HtmlNode> GetNodeByXPathJoined(string xpath)
    {
        var nodes = doc.DocumentNode.SelectNodes(xpath);
        if (nodes == null || nodes.Count == 0)
        {
            throw new ScrapingException("No node found with this xPath. The xPath is invalid.");
        }

        return nodes;
    }

    public HtmlNode GetLargestArticleNode()
    {
        var article = doc.DocumentNode.Descendants("article")
                                     .OrderByDescending(a => a.InnerText.Length)
                                     .FirstOrDefault();
        if (article == null)
        {
            throw new ScrapingException("No article found in the html document");
        }

        return article;
    }

    private async Task<string> GetHtml(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            throw new ScrapingException("Exception during request (" + ex.Message + ")");
        }
    }

    public string GetTitle()
    {
        var title = doc.DocumentNode.SelectSingleNode("//title");
        if (title == null)
        {
            throw new ScrapingException("No title found in the html document");
        }

        return title.InnerText;
    }
}