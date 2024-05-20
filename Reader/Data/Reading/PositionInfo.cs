using Reader.Modules;

namespace Reader.Data.Reading;

public class PositionInfo
{
    public PositionInfo(PositionalMethod positionalMethod, string text)
    {
        MaxPosition = positionalMethod switch
        {
            PositionalMethod.Word => (uint)TextHelper.SeparateText(TextHelper.Sanitize(text)).Count(),
            PositionalMethod.CharIndex => throw new NotImplementedException("Implement logic for splitting using CharIndex as positioning method"),
            _ => throw new NotImplementedException()
        };
    }

    public int Position { get; set; }
    public PositionalMethod PositionalMethod { get; set; }
    public uint MaxPosition { get; set; }
}
