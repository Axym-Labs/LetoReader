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
using Microsoft.AspNetCore.Components.Forms;
using Reader.Data.ProductExceptions;
using Microsoft.VisualBasic;
using Reader.Data.Storage;
using Reader.Data.Product;

namespace Reader.Modules;

public class FileHelper
{
    private static Regex trimWhitespace = new(@"\s\s+", RegexOptions.Compiled);

    public static async Task<ReaderState> ExtractFromBrowserFiles(IReadOnlyList<IBrowserFile> files)
    {
        StringBuilder sb = new();

        bool[] fileSupported = new bool[files.Count];

        foreach ((var file, int i) in files.Select((file, i) => (file, i)))
        {
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(file.Size).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var fileBytes = ms.ToArray();

                if (file.ContentType == "text/plain")
                {
                    sb.Append(Encoding.UTF8.GetString(fileBytes));
                }
                else if (file.Name.EndsWith(".epub"))
                {
                    sb.Append(ExtractFromEpub(fileBytes));
                }
                else if (file.Name.EndsWith(".md"))
                {
                    sb.Append(ExtractStringFromMdStr(Encoding.UTF8.GetString(fileBytes)));
                }
                else if (file.ContentType == "application/pdf")
                {
                    sb.Append(ExtractStringFromPDF(fileBytes));
                }
                else if (file.ContentType == "text/html")
                {
                    sb.Append(ExtractStringFromHTMLStr(Encoding.UTF8.GetString(fileBytes)));
                } else
                {
                    fileSupported[i] = false;
                    continue;
                }
                fileSupported[i] = true;
            }

            if (i < files.Count - 1)
            {
                sb.Append(Environment.NewLine + Environment.NewLine + "---" + Environment.NewLine + Environment.NewLine);
            }
        }

        if (fileSupported.All(x => !x))
        {
            throw new UnsupportedOperationException("Unsupported file type", "Supported file type are: " + ProductStorage.SupportedFileImports);
        }

        string title = String.Join(", ", files.Select(
           file => file.Name.Substring(0, file.Name.LastIndexOf('.'))
        ));

        return new ReaderState(title, sb.ToString());
    }

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

    public static string ExtractFromEpub(byte[] arr)
    {
        EpubBook book = EpubReader.Read(arr);
        return trimWhitespace.Replace(book.ToPlainText()," ");

    }

    public static string ExtractStringFromMdStr(string mdStr) {
        return ExtractStringFromHTMLStr(new Marked().Parse(mdStr));
    }
}
