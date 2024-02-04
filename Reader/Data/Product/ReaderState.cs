using Newtonsoft.Json;

namespace Reader.Data.Product;

public class ReaderState
{
    public string Title;
    public int Position = 0;
    public DateTime LastRead = DateTime.Now;
    public string Text;

    [JsonConstructor]
    public ReaderState(string title, string text, DateTime lastRead) : this(title, text) {
        LastRead = lastRead;
    }

    public ReaderState(string title, string text) {
        Title = title;
        Text = text;
    }
}
