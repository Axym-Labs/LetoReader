namespace Reader.Modules;

public static class TextHelper
{
    public static string JoinSections(string a, string b)
    {
        return a + Enumerable.Repeat(Environment.NewLine, 3) + b;
    }
}
