using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkedNet;
using HtmlAgilityPack;
using UglyToad.PdfPig;
using EpubSharp;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using System.Text.RegularExpressions;

namespace Reader.Modules;

public class FileHelper
{
    private static Regex trimWhitespace = new(@"\s\s+", RegexOptions.Compiled);


    public static string ExtractStringFromPDF(byte[] byteArr) {
        StringBuilder sb = new();

        using (var document = PdfDocument.Open(byteArr))
        {
            foreach (var page in document.GetPages())
            {
                var text = ContentOrderTextExtractor.GetText(page, true);

                sb.Append(text);
            }
        }
        return sb.ToString();
    }   

    public static string ExtractStringFromHTMLStr(string htmlstr) {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlstr);
        return doc.DocumentNode.InnerText;
    }

    public static string ExtractFromEPub(byte[] arr)
    {
        EpubBook book = EpubReader.Read(arr);
        return trimWhitespace.Replace(book.ToPlainText()," ");

    }

    public static string ExtractStringFromMdStr(string mdStr) {
        return ExtractStringFromHTMLStr(new Marked().Parse(mdStr));
    }
}
