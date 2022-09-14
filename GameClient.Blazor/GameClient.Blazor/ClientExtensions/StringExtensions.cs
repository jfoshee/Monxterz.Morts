namespace GameClient.Blazor.ClientExtensions;

public static class StringExtensions
{
    public static string? FirstLine(this string str)
    {
        using var reader = new StringReader(str);
        return reader.ReadLine();
    }
}
