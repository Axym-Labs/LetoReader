
namespace Reader.Data.Product;

using Microsoft.AspNetCore.Components.Forms;
using Reader.Modules;

public class GenericFile : IBrowserFile
{
    public string Name { get; set; }

    public DateTimeOffset LastModified { get; } = DateTime.Now;

    public long Size { get {  return ContentType.Length * 16; } }

    public string ContentType { get; set; }

    public string HexContent { get; set;  }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        byte[] content = HexHelper.StringToByteArrayFastest(HexContent);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public GenericFile(string filename, string hexContent, string contentType)
    {
        Name = filename;
        HexContent = hexContent;
        ContentType = contentType;
    }
}

