using Reader.Modules.Reading;
using System.Web;

namespace Reader.Modules;

public static class TextHelper
{
    public static string Sanitize(string text)
    {
        text = text.Replace("\r\n", "\n");
        text = text.Replace("\r", "\n");
        text = text.Replace("\n", Environment.NewLine);
        text = RemoveEmptySpaces(text);
        text = text.Trim();
        text = HttpUtility.HtmlDecode(text);

        return text;
    }
    public static string JoinSections(string a, string b)
    {
        return a + Enumerable.Repeat(Environment.NewLine, 3) + b;
    }

    public static IEnumerable<string> SeparateText(string text)
    {
        return text.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static string RemoveEmptySpaces(string text)
    {
        var textPieces = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        textPieces = textPieces.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        textPieces = textPieces = textPieces.Select(x => x.Trim()).ToArray();
        textPieces = string.Join(Environment.NewLine, textPieces).Split("" , StringSplitOptions.RemoveEmptyEntries);
        return string.Join(Environment.NewLine, textPieces);
    }

    public static string GetDisplayableReadingTime(int PieceCount, int ReadingSpeed)
    {
        float hours = (float)PieceCount / ReadingSpeed / 60;
        int minutes = (int)((hours - (int)hours) * 60);

        return $"{(int)hours}h {minutes}m";
    }
}
