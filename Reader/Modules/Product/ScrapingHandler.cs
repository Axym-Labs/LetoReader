namespace Reader.Modules.Product;

using Reader.Data.Product;
using Reader.Data.Reading;

public class ScrapingHandler
{
    public ScrapingHandler()
    {

    }

    public async Task<ReaderState> Scrape(ScrapeInputs inputs)
    {
        var websiteInfo = new WebExtractor();
        await websiteInfo.Load(inputs.Url);
        var title = websiteInfo.GetTitle();

        var text = inputs.NewTextInputMethod switch
        {
            NewTextInputMethod.LargestArticleSubsection => websiteInfo.GetLargestArticleNode().InnerText,
            NewTextInputMethod.XPath =>
            inputs.XPathInputs.SelectAll
            ? websiteInfo.GetAllNodesByXPath(inputs.XPathInputs.XPath).Select(node => node.InnerText).Aggregate(JoinSections)
            : websiteInfo.GetNodeByXPath(inputs.XPathInputs.XPath).InnerText,
            _ => throw new ScrapingException("Invalid new text input method")
        };

        return new ReaderState(title, text);
    }

    private string JoinSections(string a, string b)
    {
        return a + Enumerable.Repeat(Environment.NewLine, 3) + b;
    }
}
