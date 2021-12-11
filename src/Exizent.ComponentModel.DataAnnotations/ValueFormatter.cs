namespace Exizent.ComponentModel.DataAnnotations;

internal static class ValueFormatter
{
    public static string FormatValue(object? value)
        => value?.ToString() ?? "null";

    public static string FormatOrValues(object[] values)
        => FormatValues(values, "or");
    
    public static string FormatAndValues(object[] values)
        => FormatValues(values, "or");

    private static string FormatValues(object[] values, string joiningWord)
    {
        if (values is { Length: 1 })
            return FormatValue(values[0]);

        var formatted = string.Join(", ", values[..^1].Select(FormatValue));

        return formatted + " " + joiningWord + " " + FormatValue(values[^1]);
    }
}