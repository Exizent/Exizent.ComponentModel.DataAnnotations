namespace Exizent.ComponentModel.DataAnnotations;

internal static class ValueFormatter
{
    public static string FormatOrValues(object[] values)
        => FormatValues(values, "or");
    
    public static string FormatAndValues(object[] values)
        => FormatValues(values, "or");

    private static string FormatValues(object[] values, string joiningWord)
    {
        if (values is { Length: 1 })
            return values[0].ToString() ?? string.Empty;

        var formatted = string.Join(", ", values[..^1]);

        return formatted + " " + joiningWord + " " + values[^1];
    }
}