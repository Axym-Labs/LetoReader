namespace Reader.Data.Reading;

public class PositionInfo
{
    public PositionInfo(PositionalMethod positionalMethod, string text)
    {

    }

    public uint Position { get; set; }
    public PositionalMethod PositionalMethod { get; set; }
    public uint MaxPosition { get; set; }
}
