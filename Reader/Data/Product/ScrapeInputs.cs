namespace Reader.Data.Product;

public class XPathInputs
{
    public string XPath { get; set; } = string.Empty;

    public bool SelectAll { get; set; } = false;

    public XPathInputs()
    {

    }
}

public enum NewTextInputMethod
{
    LargestArticleSubsection,
    XPath
}

public class ScrapeInputs
{
    public string NewTextInputMethodString { get; set; } = "Largest article subsection";

    public NewTextInputMethod NewTextInputMethod => NewTextInputMethodString switch
    {
        "Largest article subsection" => NewTextInputMethod.LargestArticleSubsection,
        "XPath" => NewTextInputMethod.XPath,
        _ => NewTextInputMethod.LargestArticleSubsection
    };

    public string Url { get; set; } = string.Empty;

    public string? Html { get; set; } = null;

    public XPathInputs XPathInputs = new();

    public ScrapeInputs()
    {

    }

    public static ScrapeInputs Copy(ScrapeInputs inputs)
    {
        return new ScrapeInputs
        {
            NewTextInputMethodString = inputs.NewTextInputMethodString,
            Url = inputs.Url,
            Html = inputs.Html,
            XPathInputs = new XPathInputs
            {
                XPath = inputs.XPathInputs.XPath,
                SelectAll = inputs.XPathInputs.SelectAll
            }
        };
    }
}
