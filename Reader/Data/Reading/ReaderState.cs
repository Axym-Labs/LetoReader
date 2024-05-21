using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reader.Data.Product;
using Reader.Data.ProductExceptions;
using Reader.Data.Storage;
using Reader.Modules;
using Reader.Modules.Logging;
using Reader.Modules.Product;

namespace Reader.Data.Reading;

public class ReaderState
{
    private string _title = ProductConstants.DefaultNewTitle;
    public string Title
    {
        get => _title;
        set => _title = value.Trim();
    }
    public PositionInfo PositionInfo;
    public DateTime LastRead;
    public ReaderStateSource Source;
    public string? SourceDescription;

    public ReaderState(string title, string text,ReaderStateSource source, string? sourceDescription = null, DateTime? lastRead = null, PositionalMethod positionalMethod = PositionalMethod.Word)
    {
        Title = title;
        Source = source;
        SourceDescription = sourceDescription;
        LastRead = lastRead ?? DateTime.Now;

        PositionInfo = new PositionInfo(positionalMethod, text);
    }

    public static Tuple<ReaderState,string> ImportFromJson(JObject json, string? version = null)
    {
        JObject? stateObj = json;
        if (stateObj == null)
        {
            throw new ArgumentException("Empty Json string");
        }
        if (stateObj["Text"] == null || stateObj["Text"]!.Value<string>() == null)
        {
            throw new ArgumentException("Text is missing but required");
        }

        var text = stateObj["Text"]!.Value<string>()!;
        
        DateTime lastRead = stateObj["LastRead"]?.Value<DateTime>() ?? DateTime.Now;

        ReaderStateSource source =
            stateObj["Source"] != null 
            ? (ReaderStateSource)stateObj["Source"]!.Value<Int64>()
            : ReaderStateSource.Unknown;
        
        string? sourceDescription = stateObj["SourceDescription"]?.Value<string>();

        string title = stateObj["Title"]?.Value<string>() ?? GetNew(source, sourceDescription).Item1.Title;

        var state = new ReaderState(title, text, source, sourceDescription, lastRead);

        Log.Verbose("Imported ReaderState from Json", new { source, sourceDescription, version });

        return new Tuple<ReaderState,string>(state, text);
    }

    public static async Task<Tuple<ReaderState,string>> Scrape(ScrapeInputs inputs)
    {
        var websiteInfo = new WebExtractor();
        if (!String.IsNullOrEmpty(inputs.Html))
        {
            websiteInfo.LoadFromHtml(inputs.Html);
        } else
        {
            await websiteInfo.Load(inputs.Url);
        }
        var title = websiteInfo.GetTitle();

        var text = inputs.NewTextInputMethod switch
        {
            NewTextInputMethod.LargestArticleSubsection => websiteInfo.GetLargestArticleNode().InnerText,
            NewTextInputMethod.XPath =>
            inputs.XPathInputs.SelectAll
            ? websiteInfo.GetAllNodesByXPath(inputs.XPathInputs.XPath).Select(node => node.InnerText).Aggregate(TextHelper.JoinSections)
            : websiteInfo.GetNodeByXPath(inputs.XPathInputs.XPath).InnerText,
            _ => throw new ScrapingException("Invalid new text input method")
        };
        

        return new Tuple<ReaderState,string>(new ReaderState(title, text, ReaderStateSource.WebsiteExtract, $"Scraped from {inputs.Url}", DateTime.Now, PositionalMethod.Word),text);
    }


    public static Tuple<ReaderState,string> GetDemo(ReaderStateSource source, string? sourceDescription = null)
    {
        return new Tuple<ReaderState, string>(new ReaderState(ProductConstants.DemoTitle, ProductConstants.DemoText, source, sourceDescription, DateTime.Now), ProductConstants.DemoText);
    }

    public static Tuple<ReaderState, string> GetNew(ReaderStateSource source, string? sourceDescription = null)
    {
        return new Tuple<ReaderState, string> (new ReaderState(ProductConstants.DefaultNewTitle, ProductConstants.DefaultNewText, source, sourceDescription, DateTime.Now), ProductConstants.DefaultNewText);
    }

}
