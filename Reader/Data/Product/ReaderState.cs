using Newtonsoft.Json;
using Reader.Data.Storage;

namespace Reader.Data.Product;

public class ReaderState
{
    public string Title;
    public int Position = 0;
    public DateTime LastRead;
    public string Text;

    [JsonConstructor]
    public ReaderState(string title, string text, DateTime lastRead) : this(title, text) {
        LastRead = lastRead;
    }

    public ReaderState(string title, string text) {
        Title = title;
        Text = text;
        LastRead = DateTime.Now;
    }

    public static ReaderState GetDemo()
    {
        return new ReaderState(ProductStorage.DemoTitle, ProductStorage.DemoText);
    }

    public static ReaderState GetNew()
    {
        return new ReaderState(ProductStorage.DefaultNewTitle, ProductStorage.DefaultNewText);
    }
}
