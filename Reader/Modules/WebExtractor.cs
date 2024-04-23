using HtmlAgilityPack;

namespace Reader.Modules;

public class ScrapingException : Exception
{
    public ScrapingException(string message) : base(message)
    {

    }
}

public class WebExtractor
{
    private readonly HttpClient _httpClient;

    public WebExtractor()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> GetNodeByXPath(string url, string xpath)
    {
        return (await GetNodeByXPathJoined(url, xpath)).First();
    }

    public async Task<IEnumerable<string>> GetAllNodesByXPath(string url, string xpath)
    {
        return await GetNodeByXPathJoined(url, xpath);
    }

    public async Task<IEnumerable<string>> GetNodeByXPathJoined(string url, string xpath)
    {
        var html = await GetHtml(url);
        if (string.IsNullOrEmpty(html))
        {
            throw new ScrapingException("Empty html document");
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var nodes = doc.DocumentNode.SelectNodes(xpath);
        if (nodes == null || nodes.Count == 0)
        {
            throw new ScrapingException("No node found with this xPath. The xPath is invalid.");
        }

        return nodes.Select(n => n.InnerHtml);
    }

    public async Task<string> GetHtmlByLargestArticle(string url)
    {
        var html = await GetHtml(url);
        if (string.IsNullOrEmpty(html))
        {
            throw new ScrapingException("Empty html document");
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var articles = doc.DocumentNode.Descendants("article")
                                     .OrderByDescending(a => a.InnerHtml.Length)
                                     .FirstOrDefault();

        return articles?.InnerHtml;
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
}